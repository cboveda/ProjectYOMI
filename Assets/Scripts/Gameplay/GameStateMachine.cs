using Unity.Netcode;
using UnityEngine;
using Zenject;

public class GameStateMachine : NetworkBehaviour
{
    [SerializeField] private float _gameStartDuration;
    [SerializeField] private float _roundActiveDuration;
    [SerializeField] private float _roundResolveDuration;
    [SerializeField] private float _timer;
    private bool _timerActive = false;
    private bool _timerComplete = false;
    private CombatEvaluator _combatEvaluator;
    private float _timerMax;
    private GameBaseState _currentState;
    private GameStateFactory _states;
    private IGameUIManager _gameUIManager;
    private NetworkManager _networkManager;
    private PlayerDataCollection _players;
    private ITurnHistory _turnHistory;

    public bool TimerComplete { get {  return _timerComplete; } }
    public CombatEvaluator CombatEvaluator { get { return _combatEvaluator; } }
    public float GameStartDuration { get { return _gameStartDuration; } set { _gameStartDuration = value; } }
    public float RoundActiveDuration { get { return _roundActiveDuration; } set { _roundActiveDuration = value; } }
    public float RoundResolveDuration { get { return _roundResolveDuration; } set { _roundResolveDuration = value; } }
    public GameBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public IGameUIManager GameplayUI { get { return _gameUIManager; } }
    public PlayerDataCollection Players { get {  return _players; } }
    public ITurnHistory TurnHistory { get { return _turnHistory; } }

    [Inject]
    public void Construct(NetworkManager networkManager, IGameUIManager gameUIManager, ITurnHistory turnHistory, PlayerDataCollection players, CombatEvaluator combatEvaluator)
    {
        _combatEvaluator = combatEvaluator;
        _gameUIManager = gameUIManager;
        _networkManager = networkManager;
        _players = players;
        _turnHistory = turnHistory;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        _states = new GameStateFactory(this);
        _currentState = _states.Start();
        _currentState.EnterState();
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
}
