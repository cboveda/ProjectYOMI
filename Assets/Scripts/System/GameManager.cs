using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.Networking.Transport;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    public Dictionary<ulong, PlayerOld> Players = new Dictionary<ulong, PlayerOld>();

    public UIManager UIManager;


    public NetworkVariable<float> roundTimer = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone);
    public float RoundTimer { get { return roundTimer.Value; } set { roundTimer.Value = value; } }

    [SerializeField]
    private float roundLength;
    public float RoundLength { get; }

    private bool roundTimerEnabled;
    public bool RoundTimerEnabled { get; private set; }

    /* New Round System */






    /* New Round System */

    public static void AddPlayer(ulong uid, PlayerOld player)
    {
        Debug.Log("Adding player: " + uid);
        Instance.Players.Add(uid, player);
    }

    public static void RemovePlayer(ulong uid)
    {
        Debug.Log("Removing player: " + uid);
        Instance.Players.Remove(uid);
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        if (RoundTimerEnabled && IsServer)
        {
            RoundTimer -= Time.deltaTime;
            RoundTimer = Math.Clamp(RoundTimer, 0, roundLength);
        }
    }

    public override void OnNetworkSpawn()
    {
        roundTimer.OnValueChanged += OnRoundTimerChange;
    }

    public override void OnNetworkDespawn()
    {
        roundTimer.OnValueChanged -= OnRoundTimerChange;
    }



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
        if (NetworkManager.Singleton.IsServer)
        {
            //PlayerActionButtons();
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
        GUILayout.Label("Game State: " + Enum.GetName(typeof(GameStateManager.States), GameStateManager.Instance.State));
    }

    void PlayerStatusLabels()
    {
        GUILayout.Label("Round Timer: " + RoundTimer);

        foreach (ulong uid in NetworkManager.Singleton.ConnectedClients.Keys)
        {
            GUILayout.Label($"Client {uid} move: {Enum.GetName(typeof(PlayerOld.PlayerActions), NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<PlayerOld>().PlayerAction.Value)}");
            GUILayout.Label($"Client {uid} health: {NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid).GetComponent<PlayerOld>().Health}");
        }
    }

    void PlayerActionButtons()
    {
        if (IsServer)
        {
            if (GUILayout.Button("Start")) { StartRoundTimerServerRpc(); }
            GUILayout.Space(20);
        }

        if (IsClient)
        {
            var player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<PlayerOld>();
            if (GUILayout.Button("Light Attack")) { player.LightAttack(); }
            if (GUILayout.Button("Heavy Attack")) { player.HeavyAttack(); }
            if (GUILayout.Button("Parry")) { player.Parry(); }
            if (GUILayout.Button("Grab")) { player.Grab(); }
            //if (GUILayout.Button("Health Test")) { player.ChangeHealthServerRpc(10); }
        }
    }
    #endregion

    #region RoundTimer
    [ServerRpc(RequireOwnership = false)]
    public void StartRoundTimerServerRpc()
    {
        RoundTimer = roundLength;
        RoundTimerEnabled = true;
    }

    public void OnRoundTimerChange(float oldValue, float newValue)
    {
        if (RoundTimer == 0.0f) EndOfRound();
        UIManager.Instance.SetRoundTimer(newValue);
    }

    private void EndOfRound()
    {
        RoundTimerEnabled = false;

        if (!IsServer) return;

        // Do End of Round Stuff
        Debug.Log("End of round.");

        //Fetch moves and players
        PlayerOld p1 = Players.Values.ElementAt(0);
        PlayerOld p2 = Players.Values.ElementAt(1);
        byte p1Move = p1.PlayerAction.Value;
        byte p2Move = p2.PlayerAction.Value;

        Debug.Log(p1Move + " " + p2Move);

        //Evaluate combat
        switch (p1Move)
        {
            case (byte)PlayerOld.PlayerActions.None:
                if (p2Move != (byte)PlayerOld.PlayerActions.None) p1.ChangeHealth(10);
                break;
            case (byte)PlayerOld.PlayerActions.LightAttack:
                if (p2Move == (byte)PlayerOld.PlayerActions.HeavyAttack) p2.ChangeHealth(10);
                if (p2Move == (byte)PlayerOld.PlayerActions.Grab) p2.ChangeHealth(10);
                if (p2Move == (byte)PlayerOld.PlayerActions.Parry) p1.ChangeHealth(10);
                break;
            case (byte)PlayerOld.PlayerActions.HeavyAttack:
                if (p2Move == (byte)PlayerOld.PlayerActions.LightAttack) p1.ChangeHealth(10);
                if (p2Move == (byte)PlayerOld.PlayerActions.Grab) p1.ChangeHealth(10);
                if (p2Move == (byte)PlayerOld.PlayerActions.Parry) p2.ChangeHealth(10);
                break;
            case (byte)PlayerOld.PlayerActions.Parry:
                if (p2Move == (byte)PlayerOld.PlayerActions.LightAttack) p2.ChangeHealth(10);
                if (p2Move == (byte)PlayerOld.PlayerActions.HeavyAttack) p1.ChangeHealth(10);
                if (p2Move == (byte)PlayerOld.PlayerActions.Grab) p1.ChangeHealth(10);
                break;
            case (byte)PlayerOld.PlayerActions.Grab:
                if (p2Move == (byte)PlayerOld.PlayerActions.LightAttack) p1.ChangeHealth(10);
                if (p2Move == (byte)PlayerOld.PlayerActions.HeavyAttack) p2.ChangeHealth(10);
                if (p2Move == (byte)PlayerOld.PlayerActions.Parry) p2.ChangeHealth(10);
                break;
            default:
                break;
        }

        //Clear moves
        foreach (var p in Players) p.Value.PlayerAction.Value = (byte)PlayerOld.PlayerActions.None;

        //Start next round
        StartRoundTimerServerRpc();
    }
    #endregion

    public bool CheckPlayerHealths()
    {
        foreach (PlayerOld p in Players.Values)
        {
            if (p.Health <= 0)
            {
                return true;
            }
        }
        return false;
    }

    public void ResetPlayerHealths()
    {
        ResetPlayerHealthsServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void ResetPlayerHealthsServerRpc()
    {
        foreach (PlayerOld p in Players.Values)
        {
            p.Health = 100;
        }
    }
}

