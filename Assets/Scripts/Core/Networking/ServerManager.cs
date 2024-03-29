using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine.SceneManagement;
using UnityEngine;
using Unity.Netcode.Transports.UTP;
using Zenject;

public class ServerManager : MonoBehaviour, IServerManager
{
    private NetworkManager _networkManager;
    private bool _gameHasStarted;
    public Dictionary<ulong, ClientData> ClientData { get; private set; }

    [SerializeField] private string _gameplaySceneName = "Gameplay";
    [SerializeField] private string _characterSelectScene = "CharacterSelect";
    public string JoinCode { get; private set; }

    [Inject]
    public void Construct(NetworkManager networkManager)
    {
        _networkManager = networkManager;
    }

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public async void StartHost()
    {
        Allocation allocation;
        try
        {
            allocation = await RelayService.Instance.CreateAllocationAsync(2);
        }
        catch (Exception e)
        {
            Debug.LogError($"Relay create allocation request failed, {e.Message}");
            throw;
        }

        try
        {
            JoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        }
        catch (Exception e)
        {
            Debug.LogError($"Relay get join code request failed {e.Message}");
            throw;
        }

        var relayServerData = new RelayServerData(allocation, "dtls");
        _networkManager.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

        _networkManager.ConnectionApprovalCallback += ApprovalCheck;
        _networkManager.OnServerStarted += OnNetworkReady;

        ClientData = new Dictionary<ulong, ClientData>();

        _networkManager.StartHost();
    }

    public void Disconnect()
    {
        _networkManager.Shutdown();
    }

    public void ApprovalCheck(
        NetworkManager.ConnectionApprovalRequest request,
        NetworkManager.ConnectionApprovalResponse response)
    {
        if (ClientData.Count >= 2 || _gameHasStarted)
        {
            response.Approved = false;
            return;
        }

        response.Approved = true;
        response.CreatePlayerObject = false;
        response.Pending = false;

        ClientData[request.ClientNetworkId] = new ClientData(request.ClientNetworkId);
        Debug.Log($"Added client {request.ClientNetworkId}");
    }

    private void OnNetworkReady()
    {
        _networkManager.OnClientDisconnectCallback += OnClientDisconnect;

        _networkManager.SceneManager.LoadScene(_characterSelectScene, LoadSceneMode.Single);
    }

    private void OnClientDisconnect(ulong clientId)
    {
        try
        {
            if (ClientData.ContainsKey(clientId))
            {
                if (ClientData.Remove(clientId))
                {
                    Debug.Log($"Removed client {clientId}");
                }
            }
        }
        catch
        {
            Debug.LogError("Error when removing client");
            throw;
        }
    }

    public void SetCharacter(ulong clientId, int characterId)
    {
        if (ClientData.TryGetValue(clientId, out ClientData data))
        {
            data.characterId = characterId;
        }
    }

    public void StartGame()
    {
        _gameHasStarted = true;
        StartCoroutine(DelayedSceneLoad());
    }

    public IEnumerator DelayedSceneLoad()
    {
        yield return new WaitForSeconds(1.5f);
        _networkManager.SceneManager.LoadScene(_gameplaySceneName, LoadSceneMode.Single);
    }

    public void ResetGame()
    {
        foreach (ClientData data in ClientData.Values)
        {
            data.characterId = -1;
        }

        foreach (NetworkObject obj in _networkManager.SpawnManager.SpawnedObjects.Values.ToList<NetworkObject>())
        {
            obj.Despawn();
        }

        _networkManager.SceneManager.LoadScene(_characterSelectScene, LoadSceneMode.Single);
    }
}


