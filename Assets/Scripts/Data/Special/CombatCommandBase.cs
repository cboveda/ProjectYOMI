public abstract class CombatCommandBase
{
    public ulong ClientId;
    public int Round;
    public bool HasExecuted;

    public CombatCommandBase (ulong clientId, int round)
    {
        ClientId = clientId;
        Round = round;
        HasExecuted = false;
    }

    public virtual void Execute(GameData context)
    {
        HasExecuted = true;
    }
}