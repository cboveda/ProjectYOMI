using Unity.Netcode;
using UnityEngine;
using Zenject;

public class GameStateMachine : NetworkBehaviour, IGameStateMachine
{
    [SerializeField] private float _gameStartDuration;
    [SerializeField] private float _roundActiveDuration;
    [SerializeField] private float _roundResolveDuration;
    [SerializeField] private float _timer;
    private bool _timerActive = false;
    private bool _timerComplete = false;
    private CombatConfiguration _combatConfiguration;
    private CombatCommandExecutor _combatEvaluator;
    private float _timerMax;
    private GameBaseState _currentState;
    private GameStateFactory _states;
    private IGameUIManager _gameUIManager;
    private IPlayerDataCollection _players;
    private ITurnHistory _turnHistory;
    private NetworkManager _networkManager;
    private IDatabase _database;
    private TurnFactory _turnFactory;

    public bool TimerComplete { get { return _timerComplete; } }
    public CombatConfiguration CombatConfiguration { get => _combatConfiguration; }
    public CombatCommandExecutor CombatEvaluator { get { return _combatEvaluator; } }
    public float GameStartDuration { get { return _gameStartDuration; } set { _gameStartDuration = value; } }
    public float RoundActiveDuration { get { return _roundActiveDuration; } set { _roundActiveDuration = value; } }
    public float RoundResolveDuration { get { return _roundResolveDuration; } set { _roundResolveDuration = value; } }
    public GameBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public IGameUIManager GameplayUI { get { return _gameUIManager; } }
    public IPlayerDataCollection Players { get { return _players; } }
    public ITurnHistory TurnHistory { get { return _turnHistory; } }
    public IDatabase Database { get { return _database; } }
    public TurnFactory TurnFactory { get { return _turnFactory; } }

    [Inject]
    public void Construct(
        CombatConfiguration configuration,
        CombatCommandExecutor combatEvaluator,
        IDatabase database,
        IGameUIManager gameUIManager,
        IPlayerDataCollection players,
        ITurnHistory turnHistory,
        NetworkManager networkManager,
        TurnFactory turnFactory)
    {
        _combatConfiguration = configuration;
        _combatEvaluator = combatEvaluator;
        _database = database;
        _gameUIManager = gameUIManager;
        _networkManager = networkManager;
        _players = players;
        _turnFactory = turnFactory;
        _networkManager = networkManager;
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

    void OnGUI()
    {
        if (!IsServer) return;

        GUILayout.BeginArea(new Rect(20, 20, 100, 100));

        if (GUILayout.Button("Toggle Timer"))
        {
            if (!_timerActive)
            {
                _timer = _timerMax;
            }
            _timerActive = !_timerActive;
        }        
        GUILayout.Label($"Timer: {_timer:0.0}/{_timerMax}");

        GUILayout.EndArea();
    }
}
