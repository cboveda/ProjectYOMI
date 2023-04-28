using Unity.Netcode;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking.Types;
using System;
using System.Linq;
using System.Collections.Generic;

public class GameData : NetworkBehaviour
{
    private static GameData _instance;
    public static GameData Instance { get => _instance; }

    [SerializeField] private CharacterDatabase _characterDatabase;
    [SerializeField] private CharacterMoveDatabase _characterMoveDatabase;
    [SerializeField] private GameUIManager _gameUIManager;
    [SerializeField] private int _characterIdPlayer1;
    [SerializeField] private int _characterIdPlayer2;
    [SerializeField] private NetworkVariable<byte> _usableMoveListPlayer1 = new(0);
    [SerializeField] private NetworkVariable<byte> _usableMoveListPlayer2 = new(0);
    [SerializeField] private NetworkVariable<int> _actionPlayer1 = new(-1);
    [SerializeField] private NetworkVariable<int> _actionPlayer2 = new(-1);
    [SerializeField] private NetworkVariable<int> _comboCountPlayer1 = new(0);
    [SerializeField] private NetworkVariable<int> _comboCountPlayer2 = new(0);
    [SerializeField] private NetworkVariable<int> _healthPlayer1 = new();
    [SerializeField] private NetworkVariable<int> _healthPlayer2 = new();
    [SerializeField] private NetworkVariable<int> _roundNumber = new(0);
    [SerializeField] private NetworkVariable<int> _specialMeterPlayer1 = new(0);
    [SerializeField] private NetworkVariable<int> _specialMeterPlayer2 = new(0);
    [SerializeField] private NetworkVariable<ulong> _clientIdPlayer1 = new(9999);
    [SerializeField] private NetworkVariable<ulong> _clientIdPlayer2 = new(9999);
    private RoundDataBuilder _roundDataBuilder;
    private List<RoundData> _roundDataList;
    private List<CombatCommandBase> _combatCommands;

    public int CharacterIdPlayer1 { get; set; }
    public int CharacterIdPlayer2 { get; set; }
    public NetworkVariable<byte> UsableMoveListPlayer1 { get { return _usableMoveListPlayer1; } }
    public NetworkVariable<byte> UsableMoveListPlayer2 { get { return _usableMoveListPlayer2; } }
    public NetworkVariable<int> ActionPlayer1 { get { return _actionPlayer1; } }
    public NetworkVariable<int> ActionPlayer2 { get { return _actionPlayer2; } }
    public NetworkVariable<int> ComboCountPlayer1 { get { return _comboCountPlayer1; } }
    public NetworkVariable<int> ComboCountPlayer2 { get { return _comboCountPlayer2; } }
    public NetworkVariable<int> HealthPlayer1 { get { return _healthPlayer1; } }
    public NetworkVariable<int> HealthPlayer2 { get { return _healthPlayer2; } }
    public NetworkVariable<int> RoundNumber { get { return _roundNumber; } }
    public NetworkVariable<int> SpecialMeterPlayer1 { get { return _specialMeterPlayer1; } }
    public NetworkVariable<int> SpecialMeterPlayer2 { get { return _specialMeterPlayer2; } }
    public NetworkVariable<ulong> ClientIdPlayer1 { get { return _clientIdPlayer1; } set { _clientIdPlayer1 = value; } }
    public NetworkVariable<ulong> ClientIdPlayer2 { get { return _clientIdPlayer2; } set { _clientIdPlayer2 = value; } }
    public List<CombatCommandBase> CombatCommands { get { return _combatCommands; } }
    public CharacterMoveDatabase CharacterMoveDatabase { get { return _characterMoveDatabase; } }


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

        if (!IsServer) return;
    }
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        _roundDataBuilder = new RoundDataBuilder();
        _roundDataList = new List<RoundData>();
        _combatCommands = new List<CombatCommandBase>();
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;

        _roundDataList.Clear();
    }

    public void UpdateHealthPlayer1(int delta)
    {
        _healthPlayer1.Value -= delta;
    }

    public void UpdateHealthPlayer2(int delta)
    {
        _healthPlayer2.Value -= delta;
    }

    public void InitializePlayerCharacter(int playerNumber, ulong clientId, int characterId)
    {
        Debug.Log("Called " +  playerNumber + " " + clientId);
        if (playerNumber == 1)
        {
            _clientIdPlayer1.Value = clientId;
            _characterIdPlayer1 = characterId;
            var character = _characterDatabase.GetCharacterById(_characterIdPlayer1);
            if (character != null)
            {
                _healthPlayer1.Value = character.MaximumHealth;
                foreach (CharacterMove.Type type in Enum.GetValues(typeof(CharacterMove.Type)))
                {
                    byte isUsable = (byte) (character.CharacterMoveSet.GetMoveByType(type).UsableByDefault ? 1 : 0);
                    _usableMoveListPlayer1.Value |= (byte)((byte) type * isUsable);
                }
            }
        }
        else if (playerNumber == 2)
        {
            _clientIdPlayer2.Value = clientId;
            _characterIdPlayer2 = characterId;
            var character = _characterDatabase.GetCharacterById(_characterIdPlayer2);
            if (character != null)
            {
                _healthPlayer2.Value = character.MaximumHealth;
                foreach (CharacterMove.Type type in Enum.GetValues(typeof(CharacterMove.Type)))
                {
                    byte isUsable = (byte)(character.CharacterMoveSet.GetMoveByType(type).UsableByDefault ? 1 : 0);
                    _usableMoveListPlayer2.Value |= (byte)((byte)type * isUsable);
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SubmitPlayerActionServerRpc(int moveId, ServerRpcParams serverRpcParams = default)
    {
        ulong clientId = serverRpcParams.Receive.SenderClientId;
        if (clientId == _clientIdPlayer1.Value)
        {
            _actionPlayer1.Value = moveId;
        }
        else if (clientId == _clientIdPlayer2.Value)
        {
            _actionPlayer2.Value = moveId;
        }
    }

    // This is a mess, but it's just to test functionality
    public void EvaluateRound()
    {
        _roundNumber.Value += 1;
        _roundDataBuilder.StartNewRoundData();

        // Read Inputs
        var movePlayer1 = _characterMoveDatabase.GetMoveById(_actionPlayer1.Value);
        var movePlayer2 = _characterMoveDatabase.GetMoveById(_actionPlayer2.Value);

        // Log and reset moves
        if (movePlayer1)
        {
            _roundDataBuilder.SetMoveIdPlayer1(movePlayer1.Id);
        }
        if (movePlayer2)
        {
            _roundDataBuilder.SetMoveIdPlayer2(movePlayer2.Id);
        }



        // Check for Special actions
        if (movePlayer1 && movePlayer1.MoveType == CharacterMove.Type.Special)
        {
            _characterDatabase.GetCharacterById(_characterIdPlayer1).SpecialComponent.DoSpecial(this, _clientIdPlayer1.Value);
        }
        if (movePlayer2 && movePlayer2.MoveType == CharacterMove.Type.Special)
        {
            _characterDatabase.GetCharacterById(_characterIdPlayer2).SpecialComponent.DoSpecial(this, _clientIdPlayer2.Value);
        }

        // Execute queued commands
        foreach (CombatCommandBase command in _combatCommands.Where(c => c.Round == _roundNumber.Value)) 
        {
            command.Execute(this);
        }

        // Clear executed commands
        var filtered = _combatCommands.Where(c => !c.HasExecuted).ToList();
        _combatCommands = filtered;


        // If neither submitted
        if (!movePlayer1 && !movePlayer2)
        {
            _gameUIManager.DisplayRoundResultClientRpc("None selected!", "", "None selected!", "", "Draw!");
            return;
        }
        // If one player submitted and the other didnt
        else if (movePlayer1 && !movePlayer2)
        {
            _gameUIManager.DisplayRoundResultClientRpc(
                movePlayer1.MoveName,
                Enum.GetName(typeof(CharacterMove.Type), movePlayer1.MoveType),
                "None selected!",
                "",
                "Player 1 wins round");
            _healthPlayer2.Value -= 10;
            _roundDataBuilder.SetDamageToPlayer2(-10);
        }
        else if (!movePlayer1 && movePlayer2)
        {
            _gameUIManager.DisplayRoundResultClientRpc(
                "None selected!",
                "",
                movePlayer2.MoveName,
                Enum.GetName(typeof(CharacterMove.Type), movePlayer2.MoveType),
                "Player 2 wins round");
            _healthPlayer1.Value -= 10;
            _roundDataBuilder.SetDamageToPlayer1(-10);

        }
        // Check defeat types
        else if (movePlayer1.Defeats(movePlayer2.MoveType))
        {
            _gameUIManager.DisplayRoundResultClientRpc(
                movePlayer1.MoveName,
                Enum.GetName(typeof(CharacterMove.Type), movePlayer1.MoveType),
                movePlayer2.MoveName,
                Enum.GetName(typeof(CharacterMove.Type), movePlayer2.MoveType),
                "Player 1 wins round");
            _healthPlayer2.Value -= 10;
            _roundDataBuilder.SetDamageToPlayer2(-10);

        }
        else if (movePlayer2.Defeats(movePlayer1.MoveType))
        {
            _gameUIManager.DisplayRoundResultClientRpc(
                movePlayer1.MoveName,
                Enum.GetName(typeof(CharacterMove.Type), movePlayer1.MoveType),
                movePlayer2.MoveName,
                Enum.GetName(typeof(CharacterMove.Type), movePlayer2.MoveType),
                "Player 2 wins round");
            _healthPlayer1.Value -= 10;
            _roundDataBuilder.SetDamageToPlayer1(-10);

        }
        // If both players submitted the same move type or neither wins (special case?)
        else if (movePlayer1.MoveType == movePlayer2.MoveType)
        {
            _gameUIManager.DisplayRoundResultClientRpc(
                movePlayer1.MoveName,
                Enum.GetName(typeof(CharacterMove.Type), movePlayer1.MoveType),
                movePlayer2.MoveName,
                Enum.GetName(typeof(CharacterMove.Type), movePlayer2.MoveType),
                "Draw!");

        }

        // Reset moves
        _actionPlayer1.Value = -1;
        _actionPlayer2.Value = -1;

        _roundDataList.Add(_roundDataBuilder.GetRoundData());
    }

    private void OnGUI()
    {
        if (!IsServer) return;

        GUILayout.BeginArea(new Rect(10, 10, 200, 400), GUI.skin.box);

        GUILayout.Label("Player1 usable moves: " + Convert.ToString(_usableMoveListPlayer1.Value, 2));
        foreach (CharacterMove.Type type in Enum.GetValues(typeof(CharacterMove.Type)))
        {
            if (GUILayout.Button("Toggle " + Enum.GetName(typeof(CharacterMove.Type), type)))
            {
                _usableMoveListPlayer1.Value ^= (byte)type;
            }
        }
        GUILayout.Space(10);
        GUILayout.Label("Player2 usable moves: " + Convert.ToString(_usableMoveListPlayer2.Value, 2));
        foreach (CharacterMove.Type type in Enum.GetValues(typeof(CharacterMove.Type)))
        {
            if (GUILayout.Button("Toggle " + Enum.GetName(typeof(CharacterMove.Type), type)))
            {
                _usableMoveListPlayer2.Value ^= (byte)type;
            }
        }

        GUILayout.EndArea();
    }
}
