using Zenject;

public abstract class CombatCommandBase
{
    protected PlayerDataCollection _players;
    public ulong TargetClientId;
    public int Round;
    public bool HasExecuted;

    [Inject]
    public PlayerDataCollection Players { get => _players; set => _players = value; }

    public CombatCommandBase (ulong targetClientId, int round)
    {
        TargetClientId = targetClientId;
        Round = round;
        HasExecuted = false;
    }

    public virtual void Execute()
    {
        HasExecuted = true;
    }
}