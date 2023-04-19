using UnityEngine;
using Unity.Netcode;
using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private float _notReadyDuration;
    [SerializeField] private float _gameStartDuration;
    [SerializeField] private float _roundActiveDuration;
    [SerializeField] private float _roundResolveDuration;

    [SerializeField] private float _latencyAdjustment;

    private bool _allPlayersLoaded = false;

    [SerializeField] private float _timer = 0;
    private float _timerMax = 0;
    private bool _timerActive = false;
    private bool _timerComplete = false;

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
        if (!_timerActive) return;
        _timer += Time.unscaledDeltaTime;
        if (_timer >= _timerMax)
        {
            _timerComplete = true;
            _timerActive = false;
        }
    }

    private void SetTimer(float max)
    {
        _timer = 0;
        _timerMax = max;
        _timerActive = true;
        _timerComplete = false;
    }

    private void UpdateState()
    {
        if (GameState.Instance.State == (byte)GameState.States.Init)
        {
            GameState.Instance.AdvanceState();
            return;
        }

        if (GameState.Instance.State == (byte)GameState.States.NotReady && _allPlayersLoaded)
        {
            GameState.Instance.AdvanceState();
            return;
        }

        if (_timerComplete)
        {
            GameState.Instance.AdvanceState();
            return;
        }

    }

    private void HandleNotReady()
    {
        SetTimer(_notReadyDuration);
    }

    private void HandleStartGame()
    {
        //display countdown
        SetTimer(_gameStartDuration);
    }

    private void HandleRoundActive()
    {
        GameplayUIManager.Instance.StartRoundTimer(_roundActiveDuration - _latencyAdjustment);
        SetTimer(_roundActiveDuration);
    }

    private void HandleRoundResolve()
    {
        //read user inputs
        //determine combat outcome
        //interrupt player animations
        SetTimer(_roundResolveDuration);
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
        _allPlayersLoaded = true;
        Debug.Log("All Players Loaded");
    }
}
