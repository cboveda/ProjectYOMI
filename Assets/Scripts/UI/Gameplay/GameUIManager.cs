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
    [SerializeField] private GameResult _gameResult;
    [SerializeField] private PlayerControls _playerControls;
    [SerializeField] private ProgressBar _player1Health;
    [SerializeField] private ProgressBar _player1SpecialMeter;
    [SerializeField] private ProgressBar _player2Health;
    [SerializeField] private ProgressBar _player2SpecialMeter;
    [SerializeField] private RoundResult _roundResult;
    [SerializeField] private RoundTimer _roundTimer;
    [SerializeField] private TMP_Text _player1ComboCountText;
    [SerializeField] private TMP_Text _player1Name;
    [SerializeField] private TMP_Text _player2ComboCountText;
    [SerializeField] private TMP_Text _player2Name;
    private PlayerCharacter _localPlayerCharacter;

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
        _data.TurnDataList.OnListChanged += HandleTurnData;
    }

    public override void OnNetworkDespawn()
    {
        _data.TurnDataList.OnListChanged -= HandleTurnData;
        _localPlayerCharacter.UsableMoveSet.Moves.OnValueChanged += UpdateUsableMoveButtons;
    }

    private void HandleTurnData(NetworkListEvent<TurnData> changeEvent)
    {
        Debug.Log(changeEvent.Value.ToString());

        var turnData = changeEvent.Value;
        var playerData1 = turnData.PlayerData1;
        var playerData2 = turnData.PlayerData2;
        UpdatePlayer1Health(playerData1.Health);
        UpdatePlayer2Health(playerData2.Health);
        UpdatePlayer1Special(playerData1.SpecialMeter);
        UpdatePlayer2Special(playerData2.SpecialMeter);
        UpdatePlayer1Combo(playerData1.ComboCount);
        UpdatePlayer2Combo(playerData2.ComboCount);
        DisplayRoundResult(turnData);
    }

    public void DisplayRoundResult(TurnData turnData)
    {
        var playerData1 = turnData.PlayerData1;
        var playerData2 = turnData.PlayerData2;       
        var movePlayer1 = _data.CharacterMoveDatabase.GetMoveById(playerData1.Action);
        var movePlayer2 = _data.CharacterMoveDatabase.GetMoveById(playerData2.Action);

        _roundResult.MoveNamePlayer1 = (movePlayer1) ? movePlayer1.MoveName : "None selected";
        _roundResult.MoveNamePlayer2 = (movePlayer2) ? movePlayer2.MoveName : "None selected";
        _roundResult.MoveTypePlayer1 = (movePlayer1) ?
            Enum.GetName(typeof(CharacterMove.Type), movePlayer1.MoveType) : "";
        _roundResult.MoveTypePlayer2 = (movePlayer2) ?
            Enum.GetName(typeof(CharacterMove.Type), movePlayer2.MoveType) : "";
        _roundResult.DamageToPlayer1 = GetDamageString(turnData.DamageToPlayer1);
        _roundResult.DamageToPlayer2 = GetDamageString(turnData.DamageToPlayer2);
        _roundResult.Result = turnData.Summary.ToString();
        _roundResult.gameObject.SetActive(true);
    }

    private void UpdateUsableMoveButtons(byte previousValue, byte newValue)
    {
        foreach (CharacterMove.Type type in Enum.GetValues(typeof(CharacterMove.Type)))
        {
            _playerControls.GetButtonByType(type).Button.interactable = (newValue & (byte)type) == (byte)type;
        }
    }

    [ClientRpc]
    public void UpdateActiveSelectionButtonClientRpc(int previousValue, int newValue, ClientRpcParams clientRpcParams = default)
    {
        var previousMove = _data.CharacterMoveDatabase.GetMoveById(previousValue);
        if (previousMove != null)
        {
            _playerControls.GetButtonByType(previousMove.MoveType).SetHighlight(false);
        }

        var currentMove = _data.CharacterMoveDatabase.GetMoveById(newValue);
        if (currentMove != null)
        {
            _playerControls.GetButtonByType(currentMove.MoveType).SetHighlight(true);
        }
    }

    [ClientRpc]
    public void StartRoundTimerClientRpc(float duration)
    {
        _roundTimer.StartTimer(duration);
    }

    [ClientRpc]
    private void InitializePlayerClientRpc(int playerNumber, float health, string name)
    {
        if (playerNumber == 1)
        {
            _player1Health.SetMaximum(health);
            _player1Health.SetCurrent(health);
            _player1Name.text = $"Player 1 [{name}]";
        }
        else if (playerNumber == 2)
        {
            _player2Health.SetMaximum(health);
            _player2Health.SetCurrent(health);
            _player2Name.text = $"Player 2 [{name}]";
        }
    }

    public void UpdatePlayer1Health(float newValue)
    {
        _player1Health.SetCurrent(newValue);
    }
    
    public void UpdatePlayer2Health(float newValue)
    {
        _player2Health.SetCurrent(newValue);
    }

    public void UpdatePlayer1Special(float newValue)
    {
        _player1SpecialMeter.SetCurrent(newValue);
    }

    public void UpdatePlayer2Special(float newValue)
    {
        _player2SpecialMeter.SetCurrent(newValue);
    }

    public void UpdatePlayer1Combo(int newValue)
    {
        if (newValue < 2)
        {
            _player1ComboContainer.SetActive(false);
        }
        else
        {
            _player1ComboCountText.text = $"x{newValue}";
            _player1ComboContainer.SetActive(true);
        }
    }

    public void UpdatePlayer2Combo(int newValue)
    {
        if (newValue < 2)
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
    public void DisplayGameResultClientRpc(string result)
    {
        _gameResult.Result = result;
        _gameResult.gameObject.SetActive(true);
        if (IsServer)
        {
            _gameResult.RestartButton.SetActive(true);
        }
        else
        {
            _gameResult.RestartText.SetActive(true);
        }
    }

    [ClientRpc]
    public void HideRoundResultClientRpc()
    {
        _roundResult.gameObject.SetActive(false);
    }


    public void RegisterPlayerCharacter(int playerNumber, ulong clientId)
    {
        var playerCharacter = _data.GetPlayerCharacterByClientId(clientId);
        if (playerCharacter == null) 
        {
            throw new Exception("Failed to get player character");
        }
        var maximumHealth = playerCharacter.Character.MaximumHealth;
        var displayName = playerCharacter.Character.DisplayName;
        InitializePlayerClientRpc(playerNumber, maximumHealth, displayName);

        var clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new[] { clientId },
            }
        };
        SetUpLocalActionButtonsClientRpc(clientRpcParams: clientRpcParams);
    }

    [ClientRpc]
    public void SetUpLocalActionButtonsClientRpc(ClientRpcParams clientRpcParams = default)
    {
        var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        _localPlayerCharacter = playerObject.GetComponent<PlayerCharacter>();
        var moveSet = _localPlayerCharacter.Character.CharacterMoveSet;
        _playerControls.RegisterCharacterMoveSet(
            lightId: moveSet.LightAttack.Id,
            heavyId: moveSet.HeavyAttack.Id,
            parryId: moveSet.Parry.Id,
            grabId: moveSet.Grab.Id,
            specialId: moveSet.Special.Id);
        _localPlayerCharacter.UsableMoveSet.Moves.OnValueChanged += UpdateUsableMoveButtons;
    }

    private string GetDamageString(float damageToPlayer1)
    {
        if (damageToPlayer1 > 0)
        {
            return $"-{damageToPlayer1:#.#}";
        }
        else if (damageToPlayer1 < 0)
        {
            return $"+{damageToPlayer1:#.#}";
        }
        return "";
    }
}

