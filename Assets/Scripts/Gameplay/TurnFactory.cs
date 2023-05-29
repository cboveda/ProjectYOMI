using Zenject;

public class TurnFactory
{
    private CombatConfiguration _config;
    private IDatabase _database;
    private IPlayerDataCollection _players;
    private ITurnHistory _turnHistory;

    public CombatConfiguration Config { get => _config; }
    public IDatabase Database { get => _database; }
    public IPlayerDataCollection Players { get => _players; }
    public ITurnHistory TurnHistory { get => _turnHistory; }

    [Inject]
    public void Construct(
        CombatConfiguration config,
        IDatabase database,
        IPlayerDataCollection players,
        ITurnHistory history)
    {
        _config = config;
        _database = database;
        _players = players;
        _turnHistory = history;
    }

    public Turn GetTurn()
    {
        return new Turn(this);
    }
}
