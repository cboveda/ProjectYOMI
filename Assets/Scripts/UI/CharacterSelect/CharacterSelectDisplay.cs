using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class CharacterSelectDisplay : NetworkBehaviour
{
    [SerializeField] private GameObject _characterInfoPanel;
    [SerializeField] private Transform _charactersHolder;
    [SerializeField] private TMP_Text _characterNameText;
    [SerializeField] private TMP_Text _characterInfoText;
    [SerializeField] private CharacterDatabase _characterDatabase;
    [SerializeField] private CharacterSelectButton _selectButtonPrefab;
    [SerializeField] private PlayerCard[] _playerCards;
    [SerializeField] private Transform _introSpawnPoint;
    [SerializeField] private Button _lockInButton;
    [SerializeField] private TMP_Text _joinCodeText;
    [SerializeField] private GameObject _joinCodeObject;

    private GameObject _introInstance;
    private List<CharacterSelectButton> _characterButtons;

    private NetworkList<CharacterSelectState> _players;

    private void Awake()
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
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;

            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                HandleClientConnected(client.ClientId);
            }
        }

        if (IsHost)
        {
            _joinCodeObject.SetActive(true);
            _joinCodeText.text = ServerManager.Instance.JoinCode;
        }


    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
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
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].ClientId != NetworkManager.Singleton.LocalClientId)
            {
                continue;
            }

            if (_players[i].IsLockedIn)
            {
                return;
            }

            if (_players[i].CharacterId == character.Id)
            {
                return;
            }
        }

        _characterNameText.text = character.DisplayName;
        _characterInfoText.text = character.InformationText;
        _characterInfoPanel.SetActive(true);
        if (_introInstance != null)
        {
            Destroy(_introInstance);
        }
        _introInstance = Instantiate(character.IntroPrefab, _introSpawnPoint);

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

        SelectServerRpc(character.Id);
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

    public void LockIn()
    {
        LockInServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void LockInServerRpc(ServerRpcParams serverRpcParams = default)
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].ClientId != serverRpcParams.Receive.SenderClientId)
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

        if (AllPlayersLockedIn())
        { 
            foreach(var player in _players)
            {
                ServerManager.Instance.SetCharacter(player.ClientId, player.CharacterId);
            }

            ServerManager.Instance.StartGame();
        }
    }

    private void HandlePlayersStateChanged(NetworkListEvent<CharacterSelectState> changeEvent)
    {
        if (_players.Count < 2)
        {
            _lockInButton.interactable = false;
        }
        else
        {
            _lockInButton.interactable = true;
        }

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

        foreach(var player in _players)
        {
            if (player.ClientId != NetworkManager.Singleton.LocalClientId)
            {
                continue;
            }

            if (player.IsLockedIn)
            {
                _lockInButton.interactable = false;
                foreach (var button in _characterButtons)
                {
                    if (button.IsDisabled)
                    {
                        continue;
                    }
                    button.SetDisabled();
                }
                break;
            }
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
