public class GameRoundActiveState : GameBaseState
{
    public GameRoundActiveState(GameStateMachine currentContext, GameStateFactory gameStateFactory) : base(currentContext, gameStateFactory)
    {
    }

    public override void CheckSwitchStates()
    {
        if (_context.TimerComplete)
        {
            SwitchState(_factory.RoundResolve());
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