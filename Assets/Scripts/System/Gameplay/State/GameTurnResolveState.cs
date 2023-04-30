public class GameTurnResolveState : GameBaseState
{
    public GameTurnResolveState(GameStateMachine currentContext, GameStateFactory gameStateFactory) : base(currentContext, gameStateFactory)
    {
    }

    public override void CheckSwitchStates()
    {
        if (_context.TimerComplete)
        {
            if (GameData.Instance.GameShouldEnd())
            {
                SwitchState(_factory.End());
            }
            else
            {
                SwitchState(_factory.TurnActive());
            }
        }
    }

    public override void EnterState()
    {
        _context.SetTimer(_context.RoundResolveDuration);
        GameData.Instance.EvaluateTurnCombat();
        //set time dilation
        //read player inputs
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