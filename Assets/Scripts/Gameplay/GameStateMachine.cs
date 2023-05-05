using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class GameStateMachine : NetworkBehaviour
{
    [SerializeField] private float _gameStartDuration;
    [SerializeField] private float _roundActiveDuration;
    [SerializeField] private float _roundResolveDuration;
    [SerializeField] private float _timer;
    private bool _allPlayersLoaded = false;
    private bool _timerActive = false;
    private bool _timerComplete = false;
    private float _timerMax;
    private GameBaseState _currentState;
    private GameStateFactory _states;
    private IGameUIManager _gameUIManager;
    private NetworkManager _networkManager;
    private GameData _gameData;
    private PlayerDataCollection _players;
    private CombatEvaluator _combatEvaluator;

    public bool AllPlayersLoaded { get { return _allPlayersLoaded; } }
    public bool TimerComplete { get {  return _timerComplete; } }
    public float GameStartDuration { get { return _gameStartDuration; } }
    public float RoundActiveDuration { get { return _roundActiveDuration; } }
    public float RoundResolveDuration { get { return _roundResolveDuration; } }
    public GameBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public IGameUIManager GameplayUI { get { return _gameUIManager; } }
    public GameData GameData { get { return _gameData; } }
    public PlayerDataCollection Players { get {  return _players; } }
    public CombatEvaluator CombatEvaluator { get { return _combatEvaluator; } }

    [Inject]
    public void Construct(NetworkManager networkManager, IGameUIManager gameUIManager, GameData gameData, PlayerDataCollection players, CombatEvaluator combatEvaluator)
    {
        _networkManager = networkManager;
        _gameUIManager = gameUIManager;
        _gameData = gameData;
        _players = players;
        _combatEvaluator = combatEvaluator;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        _networkManager.SceneManager.OnLoadEventCompleted += HandleLoadEventCompleted;

        _states = new GameStateFactory(this);
        _currentState = _states.NotReady();
        _currentState.EnterState();
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;

        _networkManager.SceneManager.OnLoadEventCompleted -= HandleLoadEventCompleted;
    }

    void Update()
    {
        if (!IsServer) return;

        UpdateTimer();
        _currentState.UpdateState();
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

    public void SetTimer(float max)
    {
        _timer = 0;
        _timerMax = max;
        _timerActive = true;
        _timerComplete = false;
    }

    private void HandleLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        _allPlayersLoaded = true;
    }
}
