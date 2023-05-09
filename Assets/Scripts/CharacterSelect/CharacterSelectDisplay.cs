using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Zenject;

public class CharacterSelectDisplay : NetworkBehaviour
{
    [SerializeField] private Button _lockInButton;
    [SerializeField] private CharacterDatabase _characterDatabase;
    [SerializeField] private CharacterSelectButton _selectButtonPrefab;
    [SerializeField] private GameObject _characterInfoPanel;
    [SerializeField] private GameObject _joinCodeObject;
    [SerializeField] private PlayerCard[] _playerCards;
    [SerializeField] private TMP_Text _characterInfoText;
    [SerializeField] private TMP_Text _characterNameText;
    [SerializeField] private TMP_Text _joinCodeText;
    [SerializeField] private Transform _charactersHolder;
    [SerializeField] private Transform _introSpawnPoint;
    private GameObject _introInstance;
    private IServerManager _serverManager;
    private NetworkManager _networkManager;
    private List<CharacterSelectButton> _characterButtons;
    private NetworkList<CharacterSelectState> _players;

#if UNITY_INCLUDE_TESTS
    public CharacterDatabase CharacterDatabase { get => _characterDatabase; set => _characterDatabase = value; }
    public GameObject CharacterInfoPanel { get => _characterInfoPanel; set => _characterInfoPanel = value; }
    public GameObject IntroInstance { get => _introInstance; set => _introInstance = value; }
    public List<CharacterSelectButton> CharacterButtons { get => _characterButtons; set => _characterButtons = value; }
    public NetworkList<CharacterSelectState> Players { get => _players; set => _players = value; }
    public PlayerCard[] PlayerCards { get => _playerCards; set => _playerCards = value; }
#endif

    [Inject]
    public void Construct(NetworkManager networkManager, IServerManager serverManager)
    {
        _serverManager = serverManager;
        _networkManager = networkManager;
    }

    void Awake()
    {
        _players = new();
        _characterButtons = new();
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            Character[] allCharacters = _characterDatabase.GetAllCharacters();

            foreach (Character character in allCharacters)
            {
                var selectButtonInstance = Instantiate(_selectButtonPrefab, _charactersHolder);
                selectButtonInstance.SetCharacter(this, character);
                _characterButtons.Add(selectButtonInstance);
            }

            _players.OnListChanged += HandlePlayersStateChanged;
        }


        if (IsServer)
        {
            _networkManager.OnClientConnectedCallback += HandleClientConnected;
            _networkManager.OnClientDisconnectCallback += HandleClientDisconnected;

            foreach (NetworkClient client in _networkManager.ConnectedClientsList)
            {
                HandleClientConnected(client.ClientId);
            }
        }

        if (IsHost)
        {
            _joinCodeObject.SetActive(true);
            _joinCodeText.text = _serverManager.JoinCode;
        }


    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            _networkManager.OnClientConnectedCallback -= HandleClientConnected;
            _networkManager.OnClientDisconnectCallback -= HandleClientDisconnected;
        }

        if (IsClient)
        {
            _players.OnListChanged -= HandlePlayersStateChanged;
        }
    }

    public void HandleClientConnected(ulong clientId)
    {
        _players.Add(new CharacterSelectState(clientId));
    }

    public void HandleClientDisconnected(ulong clientId)
    {
        for (int i = _players.Count - 1; i >= 0; i--)
        {
            if (_players[i].ClientId == clientId)
            {
                _players.RemoveAt(i);
            }
        }
    }

    public void Select(Character character)
    {
        if (!CanSelect(character.Id))
        {
            return;
        }
        UpdateCharacterInfoDisplay(character);
        StartIntroAnimationForSelectedCharacter(character);
        UpdateSelectionButtons(character);
        SelectServerRpc(character.Id);
    }

    public void LockIn()
    {
        LockInServerRpc();
    }

    private void UpdateSelectionButtons(Character character)
    {
        foreach (CharacterSelectButton characterButton in _characterButtons)
        {
            if (characterButton.Character.Id == character.Id)
            {
                characterButton.ShowSelected();
            }
            else
            {
                characterButton.ShowUnselected();
            }
        }
    }

    private void StartIntroAnimationForSelectedCharacter(Character character)
    {
        if (_introInstance != null)
        {
            Destroy(_introInstance);
        }
        _introInstance = Instantiate(character.IntroPrefab, _introSpawnPoint);
    }
    private void UpdateCharacterInfoDisplay(Character character)
    {
        _characterNameText.text = character.DisplayName;
        _characterInfoText.text = character.InformationText;
        _characterInfoPanel.SetActive(true);
    }

    private bool CanSelect(int characterId)
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].ClientId != _networkManager.LocalClientId)
            {
                continue;
            }

            if (_players[i].IsLockedIn)
            {
                return false;
            }

            if (_players[i].CharacterId == characterId)
            {
                return false;
            }
        }
        return true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SelectServerRpc(int characterId, ServerRpcParams serverRpcParams = default)
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].ClientId != serverRpcParams.Receive.SenderClientId)
            {
                continue;
            }

            if (!_characterDatabase.IsValidCharacterId(characterId))
            {
                return;
            }

            if (_players[i].ClientId == serverRpcParams.Receive.SenderClientId)
            {
                _players[i] = new CharacterSelectState(
                    _players[i].ClientId,
                    characterId,
                    _players[i].IsLockedIn
                );
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void LockInServerRpc(ServerRpcParams serverRpcParams = default)
    {
        LockInCharacterSelectState(serverRpcParams.Receive.SenderClientId);
        if (!AllPlayersLockedIn())
        {
            return;
        }
        SetAllCharacters();
        _serverManager.StartGame();
    }

    private void SetAllCharacters()
    {
        foreach (var player in _players)
        {
            _serverManager.SetCharacter(player.ClientId, player.CharacterId);
        }
    }

    private void LockInCharacterSelectState(ulong clientId)
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].ClientId != clientId)
            {
                continue;
            }

            if (!_characterDatabase.IsValidCharacterId(_players[i].CharacterId))
            {
                Debug.Log("Invalid character ID");
                return;
            }

            _players[i] = new CharacterSelectState(
                _players[i].ClientId,
                _players[i].CharacterId,
                true
            );
        }
    }

    private void HandlePlayersStateChanged(NetworkListEvent<CharacterSelectState> changeEvent)
    {
        UpdateLockInButtonForPlayerCount();
        UpdateLockInButtonForSelectState();
        UpdatePlayerCards();
    }

    private void UpdateLockInButtonForSelectState()
    {
        foreach (var player in _players)
        {
            if (player.ClientId != _networkManager.LocalClientId)
            {
                continue;
            }

            if (player.IsLockedIn)
            {
                _lockInButton.interactable = false;
                foreach (var button in _characterButtons)
                {
                    if (button.IsEnabled)
                    {
                        button.SetDisabled();
                    }
                }
                break;
            }
        }
    }

    private void UpdatePlayerCards()
    {
        for (int i = 0; i < _playerCards.Length; i++)
        {
            if (_players.Count > i)
            {
                _playerCards[i].UpdateDisplay(_players[i]);

                if (AllPlayersLockedIn())
                {
                    _playerCards[i].EnableIcon();
                }
            }
            else
            {
                _playerCards[i].DisableDisplay();
            }
        }
    }

    private void UpdateLockInButtonForPlayerCount()
    {
        if (_players.Count < 2)
        {
            _lockInButton.interactable = false;
        }
        else
        {
            _lockInButton.interactable = true;
        }
    }

    private bool AllPlayersLockedIn()
    {
        bool allReady = true;
        for (int i = 0; i < _players.Count; i++)
        {
            allReady = allReady && _players[i].IsLockedIn;
        }
        return allReady;
    }
}
