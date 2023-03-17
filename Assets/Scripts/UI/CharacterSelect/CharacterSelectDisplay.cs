using UnityEngine;
using Unity.Netcode;
using TMPro;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.TextCore.Text;

public class CharacterSelectDisplay : NetworkBehaviour
{
    [SerializeField] private GameObject characterInfoPanel;
    [SerializeField] private Transform charactersHolder;
    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private CharacterDatabase characterDatabase;
    [SerializeField] private CharacterSelectButton selectButtonPrefab;
    [SerializeField] private PlayerCard[] playerCards;
    [SerializeField] private Transform introSpawnPoint;
    [SerializeField] private Button lockInButton;

    private GameObject introInstance;
    private List<CharacterSelectButton> characterButtons = new List<CharacterSelectButton>();

    private NetworkList<CharacterSelectState> players;

    private void Awake()
    {
        players = new NetworkList<CharacterSelectState>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            Character[] allCharacters = characterDatabase.GetAllCharacters();

            foreach (Character character in allCharacters)
            {
                var selectButtonInstance = Instantiate(selectButtonPrefab, charactersHolder);
                selectButtonInstance.SetCharacter(this, character);
                characterButtons.Add(selectButtonInstance);
            }

            players.OnListChanged += HandlePlayersStateChanged;
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
            players.OnListChanged -= HandlePlayersStateChanged;
        }
    }

    public void HandleClientConnected(ulong clientId)
    {
        players.Add(new CharacterSelectState(clientId));
    }

    public void HandleClientDisconnected(ulong clientId)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientId == clientId)
            {
                players.RemoveAt(i);
            }
        }
    }

    public void Select(Character character)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientId != NetworkManager.Singleton.LocalClientId)
            {
                continue;
            }

            if (players[i].IsLockedIn)
            {
                return;
            }

            if (players[i].CharacterId == character.Id)
            {
                return;
            }
        }

        characterNameText.text = character.DisplayName;
        characterInfoPanel.SetActive(true);
        if (introInstance != null)
        {
            Destroy(introInstance);
        }
        introInstance = Instantiate(character.IntroPrefab, introSpawnPoint);
        SelectServerRpc(character.Id);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SelectServerRpc(int characterId, ServerRpcParams serverRpcParams = default)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientId != serverRpcParams.Receive.SenderClientId)
            {
                continue;
            }

            if (!characterDatabase.IsValidCharacterId(characterId))
            {
                return;
            }

            if (players[i].ClientId == serverRpcParams.Receive.SenderClientId)
            {
                players[i] = new CharacterSelectState(
                    players[i].ClientId,
                    characterId,
                    players[i].IsLockedIn
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
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientId != serverRpcParams.Receive.SenderClientId)
            {
                continue;
            }

            if (!characterDatabase.IsValidCharacterId(players[i].CharacterId))
            {
                Debug.Log("Invalid character ID");
                return;
            }

            players[i] = new CharacterSelectState(
                players[i].ClientId,
                players[i].CharacterId,
                true
            );

        }
    }

    private void HandlePlayersStateChanged(NetworkListEvent<CharacterSelectState> changeEvent)
    {
        if (players.Count < 2)
        {
            lockInButton.interactable = false;
        }
        else
        {
            lockInButton.interactable = true;
        }

        for (int i = 0; i < playerCards.Length; i++)
        {            
            if (players.Count > i)
            {
                playerCards[i].UpdateDisplay(players[i]);

                if (AllPlayersLockedIn())
                {
                    playerCards[i].EnableIcon();
                }
            }
            else
            {
                playerCards[i].DisableDisplay();
            }
        }

        foreach(var player in players)
        {
            if (player.ClientId != NetworkManager.Singleton.LocalClientId)
            {
                continue;
            }

            if (player.IsLockedIn)
            {
                lockInButton.interactable = false;
                foreach (var button in characterButtons)
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
        for (int i = 0; i < players.Count; i++)
        {
            allReady = allReady && players[i].IsLockedIn;
        }
        return allReady;
    }
}
