using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Linq;

public class ServerManager : MonoBehaviour
{
    public static ServerManager Instance { get; private set; }

    private bool _gameHasStarted;
    public Dictionary<ulong, ClientData> ClientData { get; private set; }

    [SerializeField] private string _gameplaySceneName = "Gameplay";
    [SerializeField] private string _characterSelectScene = "CharacterSelect";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void StartServer()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.OnServerStarted += OnNetworkReady;

        ClientData = new Dictionary<ulong, ClientData>();

        NetworkManager.Singleton.StartServer();
    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.OnServerStarted += OnNetworkReady;

        ClientData = new Dictionary<ulong, ClientData>();

        NetworkManager.Singleton.StartHost();
    }

    public void Disconnect()
    {
        NetworkManager.Singleton.Shutdown();
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
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;

        NetworkManager.Singleton.SceneManager.LoadScene(_characterSelectScene, LoadSceneMode.Single);
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if (ClientData.ContainsKey(clientId))
        {
            if (ClientData.Remove(clientId))
            {
                Debug.Log($"Removed client {clientId}");
            }
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
        NetworkManager.Singleton.SceneManager.LoadScene(_gameplaySceneName, LoadSceneMode.Single);
    }

    public void ResetGame()
    {
        foreach (ClientData data in ClientData.Values)
        {
            data.characterId = -1;
        }

        foreach (NetworkObject obj in NetworkManager.Singleton.SpawnManager.SpawnedObjects.Values.ToList<NetworkObject>())
        {
            obj.Despawn();
        }

        NetworkManager.Singleton.SceneManager.LoadScene(_characterSelectScene, LoadSceneMode.Single);
    }
}


