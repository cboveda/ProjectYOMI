using Unity.Netcode;
using UnityEngine;
using UnityEditor;

public class GameData : NetworkBehaviour
{
    [SerializeField] private NetworkVariable<int> _actionPlayer1 = new(-1);
    [SerializeField] private NetworkVariable<int> _actionPlayer2 = new(-1);
    [SerializeField] private NetworkVariable<int> _healthPlayer1 = new(100);
    [SerializeField] private NetworkVariable<int> _healthPlayer2 = new(100);
    [SerializeField] private NetworkVariable<int> _roundNumber = new(0);
    [SerializeField] private ulong _clientIdPlayer1 = ulong.MaxValue;
    [SerializeField] private ulong _clientIdPlayer2 = ulong.MaxValue;
    [SerializeField] private GameplayUIManager _gameplayUIManager;

    public int ActionPlayer1 { get { return _actionPlayer1.Value; }  }
    public int ActionPlayer2 { get { return _actionPlayer2.Value; }  }
    public int HealthPlayer1 { get { return _healthPlayer1.Value; } }
    public int HealthPlayer2 { get { return _healthPlayer2.Value; } }
    public int RoundNumber { get { return _roundNumber.Value; } }

    public ulong ClientIdPlayer1 { get; set; }
    public ulong ClientIdPlayer2 { get; set; }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        _roundNumber.OnValueChanged += HandleRoundNumberChanged;
        _healthPlayer1.OnValueChanged += HandleHealthPlayer1Changed;
        _healthPlayer2.OnValueChanged += HandleHealthPlayer2Changed;
        _actionPlayer1.OnValueChanged += HandleActionPlayer1Changed;
        _actionPlayer2.OnValueChanged += HandleActionPlayer2Changed;

        _gameplayUIManager.UpdatePlayer1HealthClientRpc(HealthPlayer1);
        _gameplayUIManager.UpdatePlayer2HealthClientRpc(HealthPlayer2);
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
    public void SubmitPlayerActionServerRpc(int moveId, ServerRpcParams serverRpcParams = default)
    {
        Debug.Log("Getting called?");
        ulong clientId = serverRpcParams.Receive.SenderClientId;
        if (clientId == _clientIdPlayer1)
        {
            _actionPlayer1.Value = moveId;
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
        _gameplayUIManager.UpdatePlayer2HealthClientRpc(newValue);
    }

    private void HandleHealthPlayer1Changed(int previousValue, int newValue)
    {
        _gameplayUIManager.UpdatePlayer1HealthClientRpc(newValue);
    }

    private void HandleRoundNumberChanged(int previousValue, int newValue)
    {

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
