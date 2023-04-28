public class GameStartState : GameBaseState
{
    public GameStartState(GameStateMachine currentContext, GameStateFactory gameStateFactory) : base(currentContext, gameStateFactory)
    {
    }

    public override void CheckSwitchStates()
    {
        if(_context.TimerComplete)
        {
            SwitchState(_factory.RoundActive());
        }
    }

    public override void EnterState()
    {
        _context.SetTimer(_context.GameStartDuration);
        GameUIManager.Instance.ForceUpdateHealthbarsClientRpc();
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
