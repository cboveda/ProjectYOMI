using Zenject;

public class TurnFactory
{
    private CombatConfiguration _config;
    private IDatabase _database;
    private IPlayerDataCollection _players;
    private ITurnHistory _turnHistory;
    private CombatCommandExecutor _combatEvaluator;

    public CombatConfiguration Config { get => _config; }
    public IDatabase Database { get => _database; }
    public IPlayerDataCollection Players { get => _players; }
    public ITurnHistory TurnHistory { get => _turnHistory; }
    public CombatCommandExecutor CombatEvaluator { get => _combatEvaluator; }

    [Inject]
    public void Construct(
        CombatConfiguration config,
        IDatabase database,
        IPlayerDataCollection players,
        ITurnHistory history,
        CombatCommandExecutor combatEvaluator)
    {
        _config = config;
        _database = database;
        _players = players;
        _turnHistory = history;
        _combatEvaluator = combatEvaluator;
    }

    public Turn GetTurn()
    {
        return new Turn(this);
    }
}
