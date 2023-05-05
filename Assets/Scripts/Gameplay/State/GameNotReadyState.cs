public class GameNotReadyState : GameBaseState
{
    public GameNotReadyState(GameStateMachine currentContext, GameStateFactory gameStateFactory) : base(currentContext, gameStateFactory)
    {
    }

    public override void CheckSwitchStates()
    {
        if (_context.AllPlayersLoaded)
        {
            SwitchState(_factory.Start());
        }
    }

    public override void EnterState()
    {
    }

    public override void ExitState()
    {
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }
}
