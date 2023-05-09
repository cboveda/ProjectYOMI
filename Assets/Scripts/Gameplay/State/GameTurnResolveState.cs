public class GameTurnResolveState : GameBaseState
{
    public GameTurnResolveState(IGameStateMachine currentContext, GameStateFactory gameStateFactory) : base(currentContext, gameStateFactory)
    {
    }

    public override void CheckSwitchStates()
    {
        if (_context.TimerComplete)
        {
            if (_context.Players.GameShouldEnd())
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
        var turnData = _context.CombatEvaluator.EvaluateTurnCombat();
        _context.TurnHistory.AddTurnData(turnData);
    }

    public override void ExitState()
    {
        _context.GameplayUI.HideRoundResultClientRpc();
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }
}