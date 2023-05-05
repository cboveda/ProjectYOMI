public class GameStartState : GameBaseState
{
    public GameStartState(GameStateMachine currentContext, GameStateFactory gameStateFactory) : base(currentContext, gameStateFactory)
    {
    }

    public override void CheckSwitchStates()
    {
        if(_context.TimerComplete)
        {
            SwitchState(_factory.TurnActive());
        }
    }

    public override void EnterState()
    {
        _context.SetTimer(_context.GameStartDuration);
    }

    public override void ExitState()
    {
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }
}
