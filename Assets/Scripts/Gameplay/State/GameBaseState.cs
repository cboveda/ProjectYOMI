using UnityEngine;
public abstract class GameBaseState
{
    protected IGameStateMachine _context;
    protected GameStateFactory _factory;

    public GameBaseState(IGameStateMachine currentContext, GameStateFactory gameStateFactory)
    {
        _context = currentContext;
        _factory = gameStateFactory;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();

    protected void SwitchState(GameBaseState newState)
    {
        ExitState();
        newState.EnterState();
        _context.CurrentState = newState;
        Debug.Log($"New State: {newState}");
    }

    public override string ToString()
    {
        return this.GetType().Name;
    }
}
