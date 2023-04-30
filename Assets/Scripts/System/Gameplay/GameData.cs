using Unity.Netcode;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using Unity.Collections;

public class GameData : NetworkBehaviour
{
    #region properties
    //Singleton
    private static GameData _instance;
    public static GameData Instance { get => _instance; }

    //Dependencies
    [SerializeField] private GameUIManager _gameUIManager;
    [SerializeField] private CharacterMoveDatabase _characterMoveDatabase;
    [SerializeField] private CharacterDatabase _characterDatabase;

    //Game Configuration
    [SerializeField] private float _baseDamage = 10f;
    [SerializeField] private float _baseSpecialGain = 25f;
    [SerializeField] private float _chipDamageModifier = 0.5f;
    [SerializeField] private float _specialGainOnLossModifier = 0.35f;

    //Character/Player information
    private ulong _clientIdPlayer2;
    private ulong _clientIdPlayer1;
    private Dictionary<ulong, PlayerCharacter> _playerCharacters;

    //General data
    private bool _displayDebugMenu = false;
    private int _turnNumber = 0;
    private List<CombatCommandBase> _combatCommands;
    private NetworkList<TurnData> _turnDataList;

    //Getters and Setters
    public CharacterMoveDatabase CharacterMoveDatabase { get { return _characterMoveDatabase; } }
    public int RoundNumber { get { return _turnNumber; } }
    public List<CombatCommandBase> CombatCommands { get { return _combatCommands; } }
    public NetworkList<TurnData> TurnDataList { get { return _turnDataList; } }
    public ulong ClientIdPlayer1 { get => _clientIdPlayer1; set => _clientIdPlayer1 = value; }
    public ulong ClientIdPlayer2 { get => _clientIdPlayer2; set => _clientIdPlayer2 = value; }

    #endregion

    #region lifecycle methods
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        _turnDataList = new();
        _playerCharacters = new();
        _combatCommands = new();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            return;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer)
        {
            return;
        }
    }
    #endregion

    #region player initialization and setters
    public void RegisterPlayerCharacter(int playerNumber, ulong clientId, PlayerCharacter playerCharacter)
    {
        if (playerNumber == 1)
        {
            ClientIdPlayer1 = clientId;
        }
        else
        {
            ClientIdPlayer2 = clientId;
        }
        _playerCharacters.Add(clientId, playerCharacter);
    }

    public PlayerCharacter GetPlayerCharacterByClientId(ulong clientId)
    {
        if (!_playerCharacters.TryGetValue(clientId, out PlayerCharacter playerCharacter))
        {
            return null;
        }
        return playerCharacter;
    }
    public PlayerCharacter GetPlayerCharacterByPlayerNumber(int playerNumber)
    {
        var clientId = playerNumber == 1 ? ClientIdPlayer1 : ClientIdPlayer2;
        if (!_playerCharacters.TryGetValue(clientId, out var playerCharacter))
        {
            return null;
        }
        return playerCharacter;
    }

    public PlayerCharacter GetPlayerCharacterByOpponentClientId(ulong clientId)
    {
        var targetId = (ClientIdPlayer1 == clientId) ? ClientIdPlayer2 : ClientIdPlayer1;

        if (!_playerCharacters.TryGetValue(targetId, out PlayerCharacter playerCharacter))
        {
            return null;
        }
        return playerCharacter;
    }

    public void UpdatePlayerHealth(int playerNumber, float delta)
    {
        var playerCharacter = GetPlayerCharacterByPlayerNumber(playerNumber);
        playerCharacter.Health -= delta;
    }


    public void UpdatePlayerSpecial(int playerNumber, float delta)
    {
        var playerCharacter = GetPlayerCharacterByPlayerNumber(playerNumber);
        playerCharacter.SpecialMeter += delta;
    }

    public void IncrementPlayerComboCounter(int playerNumber)
    {
        var playerCharacter = GetPlayerCharacterByPlayerNumber(playerNumber);
        playerCharacter.ComboCount++;
    }

    public void ResetPlayerComboCounter(int playerNumber)
    {
        var playerCharacter = GetPlayerCharacterByPlayerNumber(playerNumber);
        playerCharacter.ComboCount = 0;
    }

    public bool GameShouldEnd()
    {
        return _playerCharacters.Values.Any(pc => pc.PlayerData.Health <= 0);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SubmitPlayerActionServerRpc(int moveId, ServerRpcParams serverRpcParams = default)
    {
        ulong clientId = serverRpcParams.Receive.SenderClientId;
        var playerCharcter = GetPlayerCharacterByClientId(clientId);
        playerCharcter.Action = moveId;
    }
    #endregion

    #region combat evaluation
    public void EvaluateRound()
    {
        _turnNumber += 1;

        var playerCharacter1 = GetPlayerCharacterByPlayerNumber(1);
        var playerCharacter2 = GetPlayerCharacterByPlayerNumber(2);

        if (!playerCharacter1 || !playerCharacter2)
        {
            throw new Exception("Failed to get playerCharacter objects");
        }

        // Read Inputs
        var action1 = playerCharacter1.PlayerData.Action;
        var action2 = playerCharacter2.PlayerData.Action;
        var movePlayer1 = _characterMoveDatabase.GetMoveById(action1);
        var movePlayer2 = _characterMoveDatabase.GetMoveById(action2);
        var player1Wins = (movePlayer2 == null) ||
            (movePlayer1 && movePlayer1.Defeats(movePlayer2.MoveType));
        var player2Wins = (movePlayer1 == null) ||
            (movePlayer2 && movePlayer2.Defeats(movePlayer1.MoveType));

        // Check for specials   
        if ((movePlayer1 != null) && (movePlayer1.MoveType == CharacterMove.Type.Special))
        {
            playerCharacter1.Effect.DoSpecial(this, ClientIdPlayer1);
            playerCharacter1.SpecialMeter = 0f;
            playerCharacter1.UsableMoveSet.DisableMoveByType(CharacterMove.Type.Special);
        }

        if ((movePlayer2 != null) && (movePlayer2.MoveType == CharacterMove.Type.Special))
        {
            playerCharacter2.Effect.DoSpecial(this, ClientIdPlayer2);
            playerCharacter2.SpecialMeter = 0f;
            playerCharacter2.UsableMoveSet.DisableMoveByType(CharacterMove.Type.Special);
        }

        // Preliminary damage and special calculations
        var damageToPlayer1 = 0f;
        var damageToPlayer2 = 0f;

        var special1 = _baseSpecialGain;
        special1 *= playerCharacter1.Effect.GetSpecialMeterGainModifier(this, ClientIdPlayer1);
        special1 *= playerCharacter2.Effect.GetSpecialMeterGivenModifier(this, ClientIdPlayer2);

        var special2 = _baseSpecialGain;
        special2 *= playerCharacter2.Effect.GetSpecialMeterGainModifier(this, ClientIdPlayer2);
        special2 *= playerCharacter1.Effect.GetSpecialMeterGivenModifier(this, ClientIdPlayer1);

        // Check who won and apply updates
        if (player1Wins && !player2Wins)
        {
            damageToPlayer2 = _baseDamage;
            damageToPlayer2 *= playerCharacter1.Effect.GetOutgoingDamageModifier(this, ClientIdPlayer1);
            damageToPlayer2 *= playerCharacter2.Effect.GetIncomingDamageModifier(this, ClientIdPlayer2);
            special2 *= _specialGainOnLossModifier;

            UpdatePlayerHealth(2, damageToPlayer2);
            UpdatePlayerSpecial(1, special1);
            UpdatePlayerSpecial(2, special2);
            IncrementPlayerComboCounter(1);
            ResetPlayerComboCounter(2);
        }
        else if (!player1Wins && player2Wins)
        {
            damageToPlayer1 = _baseDamage;
            damageToPlayer1 *= playerCharacter2.Effect.GetOutgoingDamageModifier(this, ClientIdPlayer1);
            damageToPlayer1 *= playerCharacter1.Effect.GetIncomingDamageModifier(this, ClientIdPlayer2);
            special1 *= _specialGainOnLossModifier;

            UpdatePlayerHealth(1, damageToPlayer1);
            UpdatePlayerSpecial(1, special1);
            UpdatePlayerSpecial(2, special2);
            IncrementPlayerComboCounter(2);
            ResetPlayerComboCounter(1);
        }
        else
        {
            damageToPlayer1 *= _chipDamageModifier;
            damageToPlayer2 *= _chipDamageModifier;
            special1 *= _specialGainOnLossModifier;
            special2 *= _specialGainOnLossModifier;

            UpdatePlayerHealth(1, damageToPlayer1);
            UpdatePlayerHealth(2, damageToPlayer2);
            UpdatePlayerSpecial(1, special1);
            UpdatePlayerSpecial(2, special2);
        }

        // Check if specials should be enabled
        if (playerCharacter1.SpecialMeter >= 100f)
        {
            playerCharacter1.UsableMoveSet.EnableMoveByType(CharacterMove.Type.Special);
        }
        if (playerCharacter2.SpecialMeter >= 100f)
        {
            playerCharacter2.UsableMoveSet.EnableMoveByType(CharacterMove.Type.Special);
        }

        // Execute queued commands
        foreach (CombatCommandBase command in _combatCommands.Where(c => c.Round == _turnNumber))
        {
            command.Execute(this);
        }

        // Log combat
        var turnData = new TurnData
        {
            TurnNumber = _turnNumber,
            PlayerData1 = playerCharacter1.PlayerData,
            PlayerData2 = playerCharacter2.PlayerData,
            DamageToPlayer1 = damageToPlayer1,
            DamageToPlayer2 = damageToPlayer2,
            Summary = GetResultString(player1Wins, player2Wins)
        };
        _turnDataList.Add(turnData);

        // Reset moves
        playerCharacter1.ResetAction();
        playerCharacter2.ResetAction();
    }

    private FixedString32Bytes GetResultString(bool player1Win, bool player2Win)
    {
        FixedString32Bytes result = "";
        if (player1Win && player2Win)
        {
            result = "Draw!";
        }
        else if (player1Win)
        {
            result = "Player 1 Wins";
        }
        else if (player2Win)
        {
            result = "Player 2 Wins";
        }
        return result;
    }
    #endregion

    #region debug UI

    private void OnGUI()
    {
        if (!IsServer) return;
        if (GUILayout.Button("Toggle Debug Menu")) _displayDebugMenu = !_displayDebugMenu;
        if (_displayDebugMenu)
        {
            GUILayout.BeginArea(new Rect(10, 10, 200, 400), GUI.skin.box);

            //

            GUILayout.EndArea();
        }
    }
    #endregion
}
