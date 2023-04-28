using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameUIManager : NetworkBehaviour
{
    private static GameUIManager _instance;
    public static GameUIManager Instance { get => _instance; }

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
        _data.HealthPlayer1.OnValueChanged += UpdatePlayer1Health;
        _data.HealthPlayer2.OnValueChanged += UpdatePlayer2Health;
        _data.ComboCountPlayer1.OnValueChanged += UpdatePlayer1Combo;
        _data.ComboCountPlayer2.OnValueChanged += UpdatePlayer2Combo;
        _data.SpecialMeterPlayer1.OnValueChanged += UpdatePlayer1Special;
        _data.SpecialMeterPlayer2.OnValueChanged += UpdatePlayer2Special;
    }

    public override void OnNetworkDespawn()
    {
        _data.HealthPlayer1.OnValueChanged -= UpdatePlayer1Health;
        _data.HealthPlayer2.OnValueChanged -= UpdatePlayer2Health;
        _data.ComboCountPlayer1.OnValueChanged -= UpdatePlayer1Combo;
        _data.ComboCountPlayer2.OnValueChanged -= UpdatePlayer2Combo;
        _data.SpecialMeterPlayer1.OnValueChanged -= UpdatePlayer1Special;
        _data.SpecialMeterPlayer2.OnValueChanged -= UpdatePlayer2Special;
        _data.UsableMoveListPlayer1.OnValueChanged -= UpdateUsableMoveButtons;
        _data.UsableMoveListPlayer2.OnValueChanged -= UpdateUsableMoveButtons;
    }

    private void UpdateUsableMoveButtons(byte previousValue, byte newValue)
    {
        foreach (CharacterMove.Type type in Enum.GetValues(typeof(CharacterMove.Type)))
        {
            _playerControls.GetButtonByType(type).Button.interactable = (newValue & (byte)type) == (byte)type;
        }
    }

    [ClientRpc]
    public void SubscribeToUsableButtonStateClientRpc()
    {
        ulong myId = NetworkManager.Singleton.LocalClientId;
        if (myId == Data.ClientIdPlayer1.Value)
        {
            Data.UsableMoveListPlayer1.OnValueChanged += UpdateUsableMoveButtons;
        }
        else if (myId == Data.ClientIdPlayer2.Value) 
        {
            Data.UsableMoveListPlayer2.OnValueChanged += UpdateUsableMoveButtons;
        }
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
    public void ForceUpdateHealthbarsClientRpc()
    {
        _player1Health.SetCurrent(_data.HealthPlayer1.Value);
        _player2Health.SetCurrent(_data.HealthPlayer2.Value);
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

