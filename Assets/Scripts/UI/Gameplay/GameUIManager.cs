using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameUIManager : NetworkBehaviour
{
    private static GameUIManager _instance;

    [SerializeField] private GameData _data;
    [SerializeField] private GameObject _player1ComboContainer;
    [SerializeField] private GameObject _player2ComboContainer;
    [SerializeField] private PlayerControls _playerControls;
    [SerializeField] private ProgressBar _player1Health;
    [SerializeField] private ProgressBar _player2Health;
    [SerializeField] private ProgressBar _player1SpecialMeter;
    [SerializeField] private ProgressBar _player2SpecialMeter;
    [SerializeField] private RoundResult _roundResult;
    [SerializeField] private RoundTimer _roundTimer;
    [SerializeField] private TMP_Text _player1ComboCountText;
    [SerializeField] private TMP_Text _player2ComboCountText;
    [SerializeField] private TMP_Text _player1Name;
    [SerializeField] private TMP_Text _player2Name;

    public GameData Data { get => _data; }

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

    public override void OnNetworkSpawn()
    {
        Data.HealthPlayer1.OnValueChanged += UpdatePlayer1Health;
        Data.HealthPlayer2.OnValueChanged += UpdatePlayer2Health;
        Data.ComboCountPlayer1.OnValueChanged += UpdatePlayer1Combo;
        Data.ComboCountPlayer2.OnValueChanged += UpdatePlayer2Combo;
        Data.SpecialMeterPlayer1.OnValueChanged += UpdatePlayer1Special;
        Data.SpecialMeterPlayer2.OnValueChanged += UpdatePlayer2Special;
    }

    public override void OnNetworkDespawn()
    {
        Data.HealthPlayer1.OnValueChanged -= UpdatePlayer1Health;
        Data.HealthPlayer2.OnValueChanged -= UpdatePlayer2Health;
        Data.ComboCountPlayer1.OnValueChanged -= UpdatePlayer1Combo;
        Data.ComboCountPlayer2.OnValueChanged -= UpdatePlayer2Combo;
        Data.SpecialMeterPlayer1.OnValueChanged -= UpdatePlayer1Special;
        Data.SpecialMeterPlayer2.OnValueChanged -= UpdatePlayer2Special;
    }

    [ClientRpc]
    public void StartRoundTimerClientRpc(float duration)
    {
        _roundTimer.StartTimer(duration);
    }

    public void UpdatePlayer1Health(int oldValue, int newValue)
    {
        _player1Health.SetCurrent(newValue);
    }
    
    public void UpdatePlayer2Health(int oldValue, int newValue)
    {
        _player2Health.SetCurrent(newValue);
    }

    public void UpdatePlayer1Special(int oldValue, int newValue)
    {
        _player1SpecialMeter.SetCurrent(newValue);
    }

    public void UpdatePlayer2Special(int oldValue, int newValue)
    {
        _player2SpecialMeter.SetCurrent(newValue);
    }

    public void UpdatePlayer1Combo(int oldValue, int newValue)
    {
        if (newValue == 0)
        {
            _player1ComboContainer.SetActive(false);
        }
        else
        {
            _player1ComboCountText.text = $"x{newValue}";
            _player1ComboContainer.SetActive(true);
        }
    }

    public void UpdatePlayer2Combo(int oldValue, int newValue)
    {
        if (newValue == 0)
        {
            _player2ComboContainer.SetActive(false);
        }
        else
        {
            _player2ComboCountText.text = $"x{newValue}";
            _player2ComboContainer.SetActive(true);
        }
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

