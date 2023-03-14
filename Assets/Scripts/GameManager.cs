using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    #region DevelopmentUI
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 600));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            StartButtons();
        }
        else
        {
            StatusLabels();
        }
        if (NetworkManager.Singleton.IsClient)
        {
            PlayerActionButtons();
            PlayerStatusLabels();
        }
        GUILayout.EndArea();
    }


    static void StartButtons()
    {
        if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
        if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
        if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
    }

    static void StatusLabels()
    {
        var mode = NetworkManager.Singleton.IsHost ? "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";
        GUILayout.Label("Transport: " + NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
        GUILayout.Label("ClientID: " + NetworkManager.Singleton.LocalClientId);
    }

    void PlayerStatusLabels()
    {
        if (IsServer)
        {
            GUILayout.Label("Round Timer: ");

            foreach (ulong uid in NetworkManager.Singleton.ConnectedClients.Keys)
            {
                GUILayout.Label($"Client {uid} move: {Enum.GetName(typeof(Player.PlayerActions), NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<Player>().PlayerAction.Value)}");
                GUILayout.Label($"Client {uid} health: {NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<Player>().Health.Value}");
            }
        }
    }

    void PlayerActionButtons()
    {
        if (IsClient)
        {
            var player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
            if (GUILayout.Button("Light Attack")) { player.LightAttack(); }
            if (GUILayout.Button("Heavy Attack")) { player.HeavyAttack(); }
            if (GUILayout.Button("Parry")) { player.Parry(); }
            if (GUILayout.Button("Grab")) { player.Grab(); }
            if (GUILayout.Button("Health Test")) { player.ChangeHealthServerRpc(10); }
        }
    }
    #endregion
}
