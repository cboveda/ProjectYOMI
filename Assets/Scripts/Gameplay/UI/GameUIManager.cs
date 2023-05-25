using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using Zenject;

public class GameUIManager : NetworkBehaviour, IGameUIManager
{
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
    [SerializeField] private TurnHistoryContent _turnHistoryContent;
    private IDatabase _database;
    private ITurnHistory _turnHistory;
    private NetworkManager _networkManager;
    private PlayerCharacter _localPlayerCharacter;
    private IPlayerDataCollection _players;

    [Inject]
    public void Construct(NetworkManager networkManager, IDatabase database, IPlayerDataCollection players, ITurnHistory turnHistory)
    {
        _networkManager = networkManager;
        _database = database;
        _players = players;
        _turnHistory = turnHistory;
    }

    public override void OnNetworkSpawn()
    {
        _turnHistory.TurnDataList.OnListChanged += HandleTurnData;
    }

    public override void OnNetworkDespawn()
    {
        _turnHistory.TurnDataList.OnListChanged -= HandleTurnData;
        _localPlayerCharacter.UsableMoveSet.Moves.OnValueChanged -= UpdateUsableMoveButtons;
    }

    private void HandleTurnData(NetworkListEvent<TurnResult> changeEvent)
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
        //DisplayRoundResult(turnData);
        _turnHistoryContent.AddTurnHistoryRow(turnData);
        DisplayComboIndicators(turnData);
    }

    private void DisplayComboIndicators(TurnResult turnData)
    {
        var localPlayerData = (_localPlayerCharacter.PlayerNumber == 1) ? turnData.PlayerData1 : turnData.PlayerData2;
        var otherPlayerData = (_localPlayerCharacter.PlayerNumber == 1) ? turnData.PlayerData2 : turnData.PlayerData1;
        var localNextComboType = (_localPlayerCharacter.PlayerNumber == 1) ? turnData.Player1NextComboMove : turnData.Player2NextComboMove;
        var otherNextComboType = (_localPlayerCharacter.PlayerNumber == 1) ? turnData.Player2NextComboMove : turnData.Player1NextComboMove;
        var isMyCombo = localPlayerData.ComboCount > 0; 
        var isOtherCombo = otherPlayerData.ComboCount > 0;
        if (isMyCombo)
        {
            var myLastMove = _database.Moves.GetMoveById(localPlayerData.Action);
            if (myLastMove.MoveType == Move.Type.Special)
            {
                _playerControls.SetComboHighlightForAllButtonsAfterSpecial(true);
            }
            else
            {
                _playerControls.SetComboHighlight(localNextComboType, isMyCombo);
            }
        }
        else if (isOtherCombo)
        {
            var otherLastMove = _database.Moves.GetMoveById(otherPlayerData.Action);
            if (otherLastMove.MoveType == Move.Type.Special)
            {
                _playerControls.SetComboHighlightForAllButtonsAfterSpecial(false);
            }
            else
            {
                _playerControls.SetComboHighlight(otherNextComboType, isMyCombo);
            }
        }
        else
        {
            _playerControls.ClearComboHighlights();
        }
    }

    public void DisplayRoundResult(TurnResult turnData)
    {
        var playerData1 = turnData.PlayerData1;
        var playerData2 = turnData.PlayerData2;
        var movePlayer1 = _database.Moves.GetMoveById(playerData1.Action);
        var movePlayer2 = _database.Moves.GetMoveById(playerData2.Action);

        _roundResult.MoveNamePlayer1 = (movePlayer1) ? movePlayer1.MoveName : "None selected";
        _roundResult.MoveNamePlayer2 = (movePlayer2) ? movePlayer2.MoveName : "None selected";
        _roundResult.MoveTypePlayer1 = (movePlayer1) ?
            Enum.GetName(typeof(Move.Type), movePlayer1.MoveType) : "";
        _roundResult.MoveTypePlayer2 = (movePlayer2) ?
            Enum.GetName(typeof(Move.Type), movePlayer2.MoveType) : "";
        _roundResult.DamageToPlayer1 = GetDamageString(turnData.DamageToPlayer1);
        _roundResult.DamageToPlayer2 = GetDamageString(turnData.DamageToPlayer2);
        _roundResult.Result = turnData.Summary.ToString();
        _roundResult.gameObject.SetActive(true);
    }

    private void UpdateUsableMoveButtons(byte previousValue, byte newValue)
    {
        foreach (Move.Type type in Enum.GetValues(typeof(Move.Type)))
        {
            var button = _playerControls.GetButtonByType(type);
            if (button != null)
            {
                button.Button.interactable = (newValue & (byte)type) == (byte)type;
            }
        }
    }

    [ClientRpc]
    public void UpdateActiveSelectionButtonClientRpc(int previousValue, int newValue, ClientRpcParams clientRpcParams = default)
    {
        var previousMove = _database.Moves.GetMoveById(previousValue);
        if (previousMove != null)
        {
            _playerControls.GetButtonByType(previousMove.MoveType).SetHighlight(false);
        }

        var currentMove = _database.Moves.GetMoveById(newValue);
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
        _localPlayerCharacter.SubmitPlayerActionServerRpc(id);
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
        var playerCharacter = _players.GetByClientId(clientId);
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
        var playerObject = _networkManager.SpawnManager.GetLocalPlayerObject();
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

