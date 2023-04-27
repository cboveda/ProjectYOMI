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

    [SerializeField] private NetworkVariable<int> _actionPlayer1 = new(-1);
    [SerializeField] private NetworkVariable<int> _actionPlayer2 = new(-1);
    [SerializeField] private NetworkVariable<int> _healthPlayer1 = new();
    [SerializeField] private NetworkVariable<int> _healthPlayer2 = new();
    [SerializeField] private NetworkVariable<int> _comboCountPlayer1 = new(0);
    [SerializeField] private NetworkVariable<int> _comboCountPlayer2 = new(0);
    [SerializeField] private NetworkVariable<int> _specialMeterPlayer1 = new(0);
    [SerializeField] private NetworkVariable<int> _specialMeterPlayer2 = new(0);
    [SerializeField] private NetworkVariable<int> _roundNumber = new(0);

    [SerializeField] private NetworkList<bool> _usableMoveListPlayer1 = new();
    [SerializeField] private NetworkList<bool> _usableMoveListPlayer2 = new();


    [SerializeField] private ulong _clientIdPlayer1;
    [SerializeField] private ulong _clientIdPlayer2;
    [SerializeField] private GameUIManager _gameUIManager;
    [SerializeField] private CharacterMoveDatabase _characterMoveDatabase;
    [SerializeField] private CharacterDatabase _characterDatabase;
    private RoundDataBuilder _roundDataBuilder;
    private List<RoundData> _roundDataList;

    public static GameData Instance { get => _instance; }
    public NetworkVariable<int> ActionPlayer1 { get { return _actionPlayer1; } }
    public NetworkVariable<int> ActionPlayer2 { get { return _actionPlayer2; } }
    public NetworkVariable<int> HealthPlayer1 { get { return _healthPlayer1; } }
    public NetworkVariable<int> HealthPlayer2 { get { return _healthPlayer2; } }
    public NetworkVariable<int> ComboCountPlayer1 { get { return _comboCountPlayer1; } }
    public NetworkVariable<int> ComboCountPlayer2 { get { return _comboCountPlayer2; } }
    public NetworkVariable<int> SpecialMeterPlayer1 { get { return _specialMeterPlayer1; } }
    public NetworkVariable<int> SpecialMeterPlayer2 { get { return _specialMeterPlayer2; } }

    public NetworkVariable<int> RoundNumber { get { return _roundNumber; } }
    public ulong ClientIdPlayer1 { get { return _clientIdPlayer1; } set { _clientIdPlayer1 = value; } }
    public ulong ClientIdPlayer2 { get { return _clientIdPlayer2; } set { _clientIdPlayer2 = value; } }


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
        if (!IsServer) return;

        _roundDataBuilder = new RoundDataBuilder();
        _roundDataList = new List<RoundData>();
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

    [ServerRpc(RequireOwnership = false)]
    public void InitializeHealthServerRpc(int value, ServerRpcParams serverRpcParams = default)
    {
        ulong clientId = serverRpcParams.Receive.SenderClientId;
        if (clientId == _clientIdPlayer1)
        {
            _healthPlayer1.Value = value;
            return;
        }
        if (clientId == _clientIdPlayer2)
        {
            _healthPlayer2.Value = value;
        }
    }


    [ServerRpc(RequireOwnership = false)]
    public void SubmitPlayerActionServerRpc(int moveId, ServerRpcParams serverRpcParams = default)
    {
        ulong clientId = serverRpcParams.Receive.SenderClientId;
        if (clientId == _clientIdPlayer1)
        {
            _actionPlayer1.Value = moveId;
            return;
        }
        if (clientId == _clientIdPlayer2)
        {
            _actionPlayer2.Value = moveId;
        }
    }

    // This is a mess, but it's just to test functionality
    public void EvaluateRound()
    {
        _roundDataBuilder.StartNewRoundData();

        var player1Move = _characterMoveDatabase.GetMoveById(_actionPlayer1.Value);
        var player2Move = _characterMoveDatabase.GetMoveById(_actionPlayer2.Value);

        if (player1Move)
        {
            _roundDataBuilder.SetMoveIdPlayer1(player1Move.Id);
        }
        if (player2Move)
        {
            _roundDataBuilder.SetMoveIdPlayer2(player2Move.Id);
        }

        _actionPlayer1.Value = -1;
        _actionPlayer2.Value = -1;

        // If neither submitted
        if (!player1Move && !player2Move)
        {
            _gameUIManager.DisplayRoundResultClientRpc("None selected!", "", "None selected!", "", "Draw!");
            return;
        }
        // If one player submitted and the other didnt
        else if (player1Move && !player2Move)
        {
            _gameUIManager.DisplayRoundResultClientRpc(
                player1Move.MoveName,
                Enum.GetName(typeof(CharacterMove.Type), player1Move.MoveType),
                "None selected!",
                "",
                "Player 1 wins round");
            _healthPlayer2.Value -= 10;
            _roundDataBuilder.SetDamageToPlayer2(-10);
        }
        else if (!player1Move && player2Move)
        {
            _gameUIManager.DisplayRoundResultClientRpc(
                "None selected!",
                "",
                player2Move.MoveName,
                Enum.GetName(typeof(CharacterMove.Type), player2Move.MoveType),
                "Player 2 wins round");
            _healthPlayer1.Value -= 10;
            _roundDataBuilder.SetDamageToPlayer1(-10);

        }
        // Check defeat types
        else if (player1Move.Defeats(player2Move.MoveType))
        {
            _gameUIManager.DisplayRoundResultClientRpc(
                player1Move.MoveName,
                Enum.GetName(typeof(CharacterMove.Type), player1Move.MoveType),
                player2Move.MoveName,
                Enum.GetName(typeof(CharacterMove.Type), player2Move.MoveType),
                "Player 1 wins round");
            _healthPlayer2.Value -= 10;
            _roundDataBuilder.SetDamageToPlayer2(-10);

        }
        else if (player2Move.Defeats(player1Move.MoveType))
        {
            _gameUIManager.DisplayRoundResultClientRpc(
                player1Move.MoveName,
                Enum.GetName(typeof(CharacterMove.Type), player1Move.MoveType),
                player2Move.MoveName,
                Enum.GetName(typeof(CharacterMove.Type), player2Move.MoveType),
                "Player 2 wins round");
            _healthPlayer1.Value -= 10;
            _roundDataBuilder.SetDamageToPlayer1(-10);

        }
        // If both players submitted the same move type or neither wins (special case?)
        else if (player1Move.MoveType == player2Move.MoveType)
        {
            _gameUIManager.DisplayRoundResultClientRpc(
                player1Move.MoveName,
                Enum.GetName(typeof(CharacterMove.Type), player1Move.MoveType),
                player2Move.MoveName,
                Enum.GetName(typeof(CharacterMove.Type), player2Move.MoveType),
                "Draw!");

        }

        _roundDataList.Add(_roundDataBuilder.GetRoundData());
    }

    private void OnGUI()
    {
        if (!IsServer) return;

        if (GUILayout.Button("Update P1 Health")) UpdateHealthPlayer1(10);
        if (GUILayout.Button("Update P2 Health")) UpdateHealthPlayer2(10);
        if (GUILayout.Button("Increase P1 Combo")) _comboCountPlayer1.Value += 1;
        if (GUILayout.Button("Reset P1 Combo")) _comboCountPlayer1.Value = 0;
        if (GUILayout.Button("Increase P1 Special")) _specialMeterPlayer1.Value += 10;
        GUILayout.Label("Player 1 Move: " + _actionPlayer1.Value);
        GUILayout.Label("Player 2 Move: " + _actionPlayer2.Value);
        GUILayout.Space(10);
        GUILayout.Label("Round History");
        foreach(RoundData roundData in _roundDataList)
        {
            GUILayout.Label($"{roundData.MoveIdPlayer1}, {roundData.MoveIdPlayer2}, {roundData.DamageToPlayer1}, {roundData.DamageToPlayer2}");
        }
    }
}
