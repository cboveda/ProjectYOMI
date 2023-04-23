using Unity.Netcode;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking.Types;
using System;
using System.Linq;

public class GameData : NetworkBehaviour
{
    private static GameData _instance;

    [SerializeField] private NetworkVariable<int> _actionPlayer1 = new(-1);
    [SerializeField] private NetworkVariable<int> _actionPlayer2 = new(-1);
    [SerializeField] private NetworkVariable<int> _healthPlayer1 = new();
    [SerializeField] private NetworkVariable<int> _healthPlayer2 = new();
    [SerializeField] private NetworkVariable<int> _roundNumber = new(0);
    [SerializeField] private ulong _clientIdPlayer1;
    [SerializeField] private ulong _clientIdPlayer2;
    [SerializeField] private GameUIManager _gameUIManager;
    [SerializeField] private CharacterMoveDatabase _characterMoveDatabase;

    public static GameData Instance { get => _instance; }
    public int ActionPlayer1 { get { return _actionPlayer1.Value; } }
    public int ActionPlayer2 { get { return _actionPlayer2.Value; } }
    public int HealthPlayer1 { get { return _healthPlayer1.Value; } }
    public int HealthPlayer2 { get { return _healthPlayer2.Value; } }
    public int RoundNumber { get { return _roundNumber.Value; } }
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

        _roundNumber.OnValueChanged += HandleRoundNumberChanged;
        _healthPlayer1.OnValueChanged += HandleHealthPlayer1Changed;
        _healthPlayer2.OnValueChanged += HandleHealthPlayer2Changed;
        _actionPlayer1.OnValueChanged += HandleActionPlayer1Changed;
        _actionPlayer2.OnValueChanged += HandleActionPlayer2Changed;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;

        _roundNumber.OnValueChanged -= HandleRoundNumberChanged;
        _healthPlayer1.OnValueChanged -= HandleHealthPlayer1Changed;
        _healthPlayer2.OnValueChanged -= HandleHealthPlayer2Changed;
        _actionPlayer1.OnValueChanged -= HandleActionPlayer1Changed;
        _actionPlayer2.OnValueChanged -= HandleActionPlayer2Changed;
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
            _gameUIManager.UpdatePlayer1HealthClientRpc(value);
            return;
        }
        if (clientId == _clientIdPlayer2)
        {
            _healthPlayer2.Value = value;
            _gameUIManager.UpdatePlayer2HealthClientRpc(value);
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

    private void HandleActionPlayer2Changed(int previousValue, int newValue)
    {

    }

    private void HandleActionPlayer1Changed(int previousValue, int newValue)
    {

    }

    private void HandleHealthPlayer2Changed(int previousValue, int newValue)
    {
        _gameUIManager.UpdatePlayer2HealthClientRpc(newValue);
    }

    private void HandleHealthPlayer1Changed(int previousValue, int newValue)
    {
        _gameUIManager.UpdatePlayer1HealthClientRpc(newValue);
    }

    private void HandleRoundNumberChanged(int previousValue, int newValue)
    {

    }

    // This is a mess, but it's just to test functionality
    public void EvaluateRound()
    {
        var player1Move = _characterMoveDatabase.GetMoveById(_actionPlayer1.Value);
        var player2Move = _characterMoveDatabase.GetMoveById(_actionPlayer2.Value);

        _actionPlayer1.Value = -1;
        _actionPlayer2.Value = -1;


        // If neither submitted
        if (!player1Move && !player2Move)
        {
            _gameUIManager.DisplayRoundResultClientRpc("None selected!", "", "None selected!", "", "Draw!");
            return;
        }

        // If one player submitted and the other didnt
        if (player1Move && !player2Move)
        {
            _gameUIManager.DisplayRoundResultClientRpc(
                player1Move.MoveName,
                Enum.GetName(typeof(CharacterMove.Type), player1Move.MoveType),
                "None selected!",
                "",
                "Player 1 wins round");
            _healthPlayer2.Value -= 10;
            return;
        }

        if (!player1Move && player2Move)
        {
            _gameUIManager.DisplayRoundResultClientRpc(
                "None selected!",
                "",
                player2Move.MoveName,
                Enum.GetName(typeof(CharacterMove.Type), player2Move.MoveType),
                "Player 2 wins round");
            _healthPlayer1.Value -= 10;
            return;
        }

        // Check defeat types
        if (player1Move.Defeats(player2Move.MoveType))
        {
            _gameUIManager.DisplayRoundResultClientRpc(
                player1Move.MoveName,
                Enum.GetName(typeof(CharacterMove.Type), player1Move.MoveType),
                player2Move.MoveName,
                Enum.GetName(typeof(CharacterMove.Type), player2Move.MoveType),
                "Player 1 wins round");
            _healthPlayer2.Value -= 10;
            return;
        }

        if (player2Move.Defeats(player1Move.MoveType))
        {
            _gameUIManager.DisplayRoundResultClientRpc(
                player1Move.MoveName,
                Enum.GetName(typeof(CharacterMove.Type), player1Move.MoveType),
                player2Move.MoveName,
                Enum.GetName(typeof(CharacterMove.Type), player2Move.MoveType),
                "Player 2 wins round");
            _healthPlayer1.Value -= 10;
            return;
        }

        // If both players submitted the same move type or neither wins (special case?)
        if (player1Move.MoveType == player2Move.MoveType)
        {
            _gameUIManager.DisplayRoundResultClientRpc(
                player1Move.MoveName,
                Enum.GetName(typeof(CharacterMove.Type), player1Move.MoveType),
                player2Move.MoveName,
                Enum.GetName(typeof(CharacterMove.Type), player2Move.MoveType),
                "Draw!");
            return;
        }
    }

    private void OnGUI()
    {
        if (!IsServer) return;

        if (GUILayout.Button("Update P1 Health")) UpdateHealthPlayer1(10);
        if (GUILayout.Button("Update P2 Health")) UpdateHealthPlayer2(10);
        GUILayout.Label("Player 1 Move: " + _actionPlayer1.Value);
        GUILayout.Label("Player 2 Move: " + _actionPlayer2.Value);
    }
}
