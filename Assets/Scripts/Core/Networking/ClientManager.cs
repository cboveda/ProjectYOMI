using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using Zenject;

public class ClientManager : MonoBehaviour, IClientManager
{
    private NetworkManager _networkManager;

    [Inject]
    public void Construct(NetworkManager networkManager)
    {
        _networkManager = networkManager;
    }


    public async void StartClient(string joinCode, Action<string> callback)
    {
        JoinAllocation allocation;

        try
        {
            allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        }
        catch (Exception e)
        {
            Debug.LogError($"Relay get join code request failed {e.Message}");
            callback("Failed to connect.");
            throw;
        }
        callback("Connecting...");
        var relayServerData = new RelayServerData(allocation, "dtls");
        _networkManager.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
        _networkManager.StartClient();
    }
}
