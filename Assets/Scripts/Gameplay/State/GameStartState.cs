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
        //show start countdown
    }

    public override void ExitState()
    {
        //destroy start countdown
    }

    public override void UpdateState()
    {
        //tween countdown?
        CheckSwitchStates();
    }
}
