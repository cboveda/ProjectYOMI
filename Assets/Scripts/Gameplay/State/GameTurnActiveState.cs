public class GameTurnActiveState : GameBaseState
{
    public GameTurnActiveState(IGameStateMachine currentContext, GameStateFactory gameStateFactory) : base(currentContext, gameStateFactory)
    {
    }

    public override void CheckSwitchStates()
    {
        if (_context.TimerComplete)
        {
            SwitchState(_factory.TurnResolve());
        }
    }

    public override void EnterState()
    {
        _context.SetTimer(_context.RoundActiveDuration);
        _context.GameplayUI.StartRoundTimerClientRpc(_context.RoundActiveDuration);
    }

    public override void ExitState()
    {

    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }
}