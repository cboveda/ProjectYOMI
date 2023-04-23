using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameUIManager : NetworkBehaviour
{
    private static GameUIManager _instance;

    [SerializeField] private TMP_Text _player1Name;
    [SerializeField] private TMP_Text _player2Name;

    [SerializeField] private ProgressBar _player1Health;
    [SerializeField] private ProgressBar _player2Health;

    [SerializeField] private RoundTimer _roundTimer;

    [SerializeField] private PlayerControls _playerControls;
    [SerializeField] private RoundResult _roundResult;

    [SerializeField] private GameData _data;


    public GameData Data { get => _data; set => _data = value; }

    public static GameUIManager Instance { get => _instance; }

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

    public void SubmitPlayerAction(int id)
    {
        Data.SubmitPlayerActionServerRpc(id);
    }

    [ClientRpc]
    public void DisplayRoundResultClientRpc(string moveNamePlayer1, string moveTypePlayer1, string moveNamePlayer2, string moveTypePlayer2, string result)
    {
        _roundResult.MoveNamePlayer1 = moveNamePlayer1;
        _roundResult.MoveNamePlayer2 = moveNamePlayer2;
        _roundResult.MoveTypePlayer1 = moveTypePlayer1;
        _roundResult.MoveTypePlayer2 = moveTypePlayer2;
        _roundResult.Result = result;
        _roundResult.gameObject.SetActive(true);
    }

    [ClientRpc]
    public void HideRoundResultClientRpc()
    {
        _roundResult.gameObject.SetActive(false);
    }
}

