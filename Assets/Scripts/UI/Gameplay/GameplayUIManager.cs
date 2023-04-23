using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameplayUIManager : NetworkBehaviour
{
    private static GameplayUIManager _instance;

    [SerializeField] private TMP_Text _player1Name;
    [SerializeField] private TMP_Text _player2Name;

    [SerializeField] private ProgressBar _player1Health;
    [SerializeField] private ProgressBar _player2Health;

    [SerializeField] private RoundTimer _roundTimer;

    [SerializeField] private PlayerControls _playerControls;

    [SerializeField] private GameData _data;

    public GameData Data { get; set; }

    public static GameplayUIManager Instance { get; }

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

    [ClientRpc]
    public void StartRoundTimerClientRpc(float duration)
    {
        _roundTimer.StartTimer(duration);
    }

    [ClientRpc]
    public void UpdatePlayer1HealthClientRpc(int newValue)
    {
        _player1Health.SetCurrent(newValue);
    }
    
    [ClientRpc]
    public void UpdatePlayer2HealthClientRpc(int newValue)
    {
        _player2Health.SetCurrent(newValue);
    }

    internal void SubmitPlayerAction(int id)
    {
        if (Data == null)
        {
            Debug.Log("Nope...");
            return;
        }
        Data.SubmitPlayerActionServerRpc(id);
    }
}

