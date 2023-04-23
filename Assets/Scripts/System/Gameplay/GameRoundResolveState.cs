public class GameRoundResolveState : GameBaseState
{
    public GameRoundResolveState(GameStateMachine currentContext, GameStateFactory gameStateFactory) : base(currentContext, gameStateFactory)
    {
    }

    public override void CheckSwitchStates()
    {
        if (_context.TimerComplete)
        {
            //add end game check here

            SwitchState(_factory.RoundActive());
        }
    }

    public override void EnterState()
    {
        _context.SetTimer(_context.RoundResolveDuration);
        //set time dilation
        //read player inputs
        GameData.Instance.EvaluateRound();
        //determine combat outcome
        //apply health changes
        //start animations
    }

    public override void ExitState()
    {
        GameUIManager.Instance.HideRoundResultClientRpc();
        //reset time dilation
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }
}