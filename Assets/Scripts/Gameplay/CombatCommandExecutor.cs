using System.Collections.Generic;
using System.Linq;
using Zenject;

public class CombatCommandExecutor
{
    private IDatabase _database;
    private IPlayerDataCollection _players;
    private ITurnHistory _turnHistory;
    private readonly List<CombatCommandBase> _combatCommands;
    public virtual IDatabase Database {  get => _database; }
    public virtual int TurnNumber { get => _turnHistory.GetCurrentTurnNumber(); }
    public virtual IPlayerDataCollection Players { get => _players; }

    public CombatCommandExecutor()
    {
        _combatCommands = new();
    }

    [Inject]
    public void Construct(
        IDatabase database,
        IPlayerDataCollection playerDataCollection,
        ITurnHistory turnHistory)
    {
        _database = database;
        _players = playerDataCollection;
        _turnHistory = turnHistory;
    }

    public virtual void AddCombatCommand(CombatCommandBase combatCommand)
    {
        _combatCommands.Add(combatCommand);
    }

    public virtual void ExecuteCombatCommands()
    {
        var commandsToExecute = _combatCommands.Where(c => c.Round == _turnHistory.GetCurrentTurnNumber());
        foreach (var command in commandsToExecute)
        {
            command.Execute(this);
        }
    }
}
