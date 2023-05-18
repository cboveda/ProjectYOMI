public abstract class CombatCommandBase
{

    public ulong TargetClientId;
    public int Round;
    public bool HasExecuted;

    public CombatCommandBase (ulong targetClientId, int round)
    {
        TargetClientId = targetClientId;
        Round = round;
        HasExecuted = false;
    }

    public virtual void Execute(CombatEvaluator context)
    {
        HasExecuted = true;
    }
}