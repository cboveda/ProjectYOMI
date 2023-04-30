using Unity.Netcode;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking.Types;
using System;
using System.Linq;
using System.Collections.Generic;

public class GameData : NetworkBehaviour
{
    #region properties
    //Singleton
    private static GameData _instance;
    public static GameData Instance { get => _instance; }

    //Dependencies
    [SerializeField] private CharacterDatabase _characterDatabase;
    [SerializeField] private CharacterMoveDatabase _characterMoveDatabase;
    [SerializeField] private GameUIManager _gameUIManager;

    //Game Configuration
    [SerializeField] private float _baseDamage = 10f;
    [SerializeField] private float _baseSpecialGain = 25f;
    [SerializeField] private float _specialGainOnLossModifier = 0.35f;
    [SerializeField] private float _chipDamageModifier = 0.5f;

    //Character/Player information
    private Dictionary<ulong, PlayerCharacter> _playerCharacters;
    private ulong _clientIdPlayer1;
    private ulong _clientIdPlayer2;

    //General data
    private RoundDataBuilder _roundDataBuilder;
    private List<CombatCommandBase> _combatCommands;
    private List<RoundData> _roundDataList;
    private int _roundNumber = 0;
    private bool _displayDebugMenu = false;

    //Getters and Setters
    public CharacterMoveDatabase CharacterMoveDatabase { get { return _characterMoveDatabase; } }
    public List<CombatCommandBase> CombatCommands { get { return _combatCommands; } }
    public List<RoundData> RoundDataList { get { return _roundDataList; } }
    public int RoundNumber { get { return _roundNumber; } }

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
    }
    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            return;
        }

        _playerCharacters = new Dictionary<ulong, PlayerCharacter>();
        _roundDataBuilder = new RoundDataBuilder();
        _roundDataList = new List<RoundData>();
        _combatCommands = new List<CombatCommandBase>();
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
        playerCharacter.PlayerData.Health -= delta;
    }


    public void UpdatePlayerSpecial(int playerNumber, float delta)
    {
        var playerCharacter = GetPlayerCharacterByPlayerNumber(playerNumber);
        playerCharacter.PlayerData.SpecialMeter += delta;
    }

    public void IncrementPlayerComboCounter(int playerNumber)
    {
        var playerCharacter = GetPlayerCharacterByPlayerNumber(playerNumber);
        playerCharacter.PlayerData.ComboCount++;
    }

    public void ResetPlayerComboCounter(int playerNumber)
    {
        var playerCharacter = GetPlayerCharacterByPlayerNumber(playerNumber);
        playerCharacter.PlayerData.ComboCount = 0;
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
        playerCharcter.PlayerData.Action = moveId;
    }
    #endregion

    #region combat evaluation
    public void EvaluateRound()
    {
        _roundNumber += 1;
        _roundDataBuilder.StartNewRoundData();

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
        var player1Wins = (movePlayer2 == null) || (movePlayer1.Defeats(movePlayer2.MoveType));
        var player2Wins = (movePlayer1 == null) || (movePlayer2.Defeats(movePlayer1.MoveType));

        // Check for specials   
        if ((movePlayer1 != null) && (movePlayer1.MoveType == CharacterMove.Type.Special))
        {
            playerCharacter1.Effect.DoSpecial(this, ClientIdPlayer1);
            playerCharacter1.PlayerData.SpecialMeter = 0f;
            playerCharacter1.UsableMoveSet.DisableMoveByType(CharacterMove.Type.Special);
        }

        if ((movePlayer2 != null) && (movePlayer2.MoveType == CharacterMove.Type.Special))
        {
            playerCharacter2.Effect.DoSpecial(this, ClientIdPlayer2);
            playerCharacter2.PlayerData.SpecialMeter = 0f;
            playerCharacter2.UsableMoveSet.DisableMoveByType(CharacterMove.Type.Special);
        }

        // Check defeat types
        if (player1Wins && !player2Wins)
        {
            _gameUIManager.DisplayRoundResultClientRpc(
                movePlayer1.MoveName,
                Enum.GetName(typeof(CharacterMove.Type), movePlayer1.MoveType),
                movePlayer2.MoveName,
                Enum.GetName(typeof(CharacterMove.Type), movePlayer2.MoveType),
                "Player 1 wins round");

            var damage = _baseDamage;
            damage *= playerCharacter1.Effect.GetOutgoingDamageModifier(this, ClientIdPlayer1);
            damage *= playerCharacter2.Effect.GetIncomingDamageModifier(this, ClientIdPlayer2);
            UpdatePlayerHealth(2, damage);

            var special1 = _baseSpecialGain;
            special1 *= playerCharacter1.Effect.GetSpecialMeterGainModifier(this, ClientIdPlayer1);
            special1 *= playerCharacter2.Effect.GetSpecialMeterGivenModifier(this, ClientIdPlayer2);
            UpdatePlayerSpecial(1, special1);

            var special2 = _baseSpecialGain;
            special2 *= playerCharacter2.Effect.GetSpecialMeterGainModifier(this, ClientIdPlayer2);
            special2 *= playerCharacter1.Effect.GetSpecialMeterGivenModifier(this, ClientIdPlayer1);
            special2 *= _specialGainOnLossModifier;
            UpdatePlayerSpecial(2, special2);

            IncrementPlayerComboCounter(1);
            ResetPlayerComboCounter(2);

            _roundDataBuilder.SetDamageToPlayer2(damage);
        }
        else if (!player1Wins && player2Wins)
        {
            _gameUIManager.DisplayRoundResultClientRpc(
                movePlayer1.MoveName,
                Enum.GetName(typeof(CharacterMove.Type), movePlayer1.MoveType),
                movePlayer2.MoveName,
                Enum.GetName(typeof(CharacterMove.Type), movePlayer2.MoveType),
                "Player 2 wins round");

            var damage = _baseDamage;
            damage *= playerCharacter2.Effect.GetOutgoingDamageModifier(this, ClientIdPlayer1);
            damage *= playerCharacter1.Effect.GetIncomingDamageModifier(this, ClientIdPlayer2);
            UpdatePlayerHealth(2, damage);

            var special1 = _baseSpecialGain;
            special1 *= playerCharacter1.Effect.GetSpecialMeterGainModifier(this, ClientIdPlayer1);
            special1 *= playerCharacter2.Effect.GetSpecialMeterGivenModifier(this, ClientIdPlayer2);
            special1 *= _specialGainOnLossModifier;
            UpdatePlayerSpecial(1, special1);

            var special2 = _baseSpecialGain;
            special2 *= playerCharacter2.Effect.GetSpecialMeterGainModifier(this, ClientIdPlayer2);
            special2 *= playerCharacter1.Effect.GetSpecialMeterGivenModifier(this, ClientIdPlayer1);
            UpdatePlayerSpecial(2, special2);

            IncrementPlayerComboCounter(2);
            ResetPlayerComboCounter(1);

            _roundDataBuilder.SetDamageToPlayer2(damage);
        }
        // If both players submitted the same move type or neither wins (special case?)
        else
        {
            _gameUIManager.DisplayRoundResultClientRpc(
                movePlayer1.MoveName,
                Enum.GetName(typeof(CharacterMove.Type), movePlayer1.MoveType),
                movePlayer2.MoveName,
                Enum.GetName(typeof(CharacterMove.Type), movePlayer2.MoveType),
                "Draw!");

            var damage = _baseDamage * _chipDamageModifier;
            UpdatePlayerHealth(1, damage);
            UpdatePlayerHealth(2, damage);

            var special1 = _baseSpecialGain;
            special1 *= playerCharacter1.Effect.GetSpecialMeterGainModifier(this, ClientIdPlayer1);
            special1 *= playerCharacter2.Effect.GetSpecialMeterGivenModifier(this, ClientIdPlayer2);
            special1 *= _specialGainOnLossModifier;
            UpdatePlayerSpecial(1, special1);

            var special2 = _baseSpecialGain;
            special2 *= playerCharacter2.Effect.GetSpecialMeterGainModifier(this, ClientIdPlayer2);
            special2 *= playerCharacter1.Effect.GetSpecialMeterGivenModifier(this, ClientIdPlayer1);
            special2 *= _specialGainOnLossModifier;
            UpdatePlayerSpecial(2, special2);

            _roundDataBuilder.SetDamageToPlayer1(damage);
            _roundDataBuilder.SetDamageToPlayer2(damage);

        }

        // Check if specials should be enabled
        if (playerCharacter1.PlayerData.SpecialMeter >= 100f)
        {
            playerCharacter1.UsableMoveSet.EnableMoveByType(CharacterMove.Type.Special);
        }
        if (playerCharacter2.PlayerData.SpecialMeter >= 100f)
        {
            playerCharacter2.UsableMoveSet.EnableMoveByType(CharacterMove.Type.Special);
        }

        // Execute queued commands
        foreach (CombatCommandBase command in _combatCommands.Where(c => c.Round == _roundNumber))
        {
            command.Execute(this);
        }

        // Log combat
        _roundDataList.Add(_roundDataBuilder.GetRoundData());

        // Reset moves
        playerCharacter1.ResetAction();
        playerCharacter2.ResetAction();
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
