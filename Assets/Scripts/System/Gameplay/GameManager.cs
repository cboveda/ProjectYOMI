using UnityEngine;
using Unity.Netcode;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private float notReadyDuration;
    [SerializeField] private float gameStartDuration;
    [SerializeField] private float roundActiveDuration;
    [SerializeField] private float roundResolveDuration;

    [SerializeField] private float latencyAdjustment;

    private bool allPlayersLoaded = false;

    [SerializeField] private float timer = 0;
    private float timerMax = 0;
    private bool timerActive = false;
    private bool timerComplete = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += HandleLoadEventCompleted;

        GameState.Instance.OnStateChangedNotReady += HandleNotReady;
        GameState.Instance.OnStateChangedStartGame += HandleStartGame;
        GameState.Instance.OnStateChangedRoundActive += HandleRoundActive;
        GameState.Instance.OnStateChangedRoundResolve += HandleRoundResolve;
        GameState.Instance.OnStateChangedEndGame += HandleEndGame;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;

        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= HandleLoadEventCompleted;

        GameState.Instance.OnStateChangedNotReady -= HandleNotReady;
        GameState.Instance.OnStateChangedStartGame -= HandleStartGame;
        GameState.Instance.OnStateChangedRoundActive -= HandleRoundActive;
        GameState.Instance.OnStateChangedRoundResolve -= HandleRoundResolve;
        GameState.Instance.OnStateChangedEndGame -= HandleEndGame;
    }

    void Update()
    {
        if (!IsServer) return;

        UpdateTimer();
        UpdateState();
    }

    private void UpdateTimer()
    {
        if (!timerActive) return;
        timer += Time.unscaledDeltaTime;
        if (timer >= timerMax)
        {
            timerComplete = true;
            timerActive = false;
        }
    }

    private void SetTimer(float max)
    {
        timer = 0;
        timerMax = max;
        timerActive = true;
        timerComplete = false;
    }

    private void UpdateState()
    {
        if (GameState.Instance.State == (byte)GameState.States.Init)
        {
            GameState.Instance.AdvanceState();
            return;
        }

        if (GameState.Instance.State == (byte)GameState.States.NotReady && allPlayersLoaded)
        {
            GameState.Instance.AdvanceState();
            return;
        }

        if (timerComplete)
        {
            GameState.Instance.AdvanceState();
            return;
        }

    }

    private void HandleNotReady()
    {
        SetTimer(notReadyDuration);
    }

    private void HandleStartGame()
    {
        //display countdown
        SetTimer(gameStartDuration);
    }

    private void HandleRoundActive()
    {
        GameplayUIManager.Instance.StartRoundTimer(roundActiveDuration - latencyAdjustment);
        SetTimer(roundActiveDuration);
    }

    private void HandleRoundResolve()
    {
        //read user inputs
        //determine combat outcome
        //interrupt player animations
        SetTimer(roundResolveDuration);
    }

    private void HandleEndGame()
    {
        //start player victory/lose animations
        //post game results
        //display "restart" modal
        Debug.Log("Entered EndGame State");
    }

    private void HandleLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        allPlayersLoaded = true;
        Debug.Log("All Players Loaded");
    }
}
