using Castle.Core.Configuration;

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
        var turn = _context.TurnFactory.GetTurn();
        var turnResult = turn
            .Initialize()
            .DetermineWinner()
            .DetermineComboStatus()
            .CheckForSpecialMovesAndExecute()
            .CalculateStateChanges()
            .ApplyStateChanges()
            .ExecuteCombatCommands()
            .CheckAndSetSpecialUsability()
            .GetTurnData();
        _context.TurnHistory.AddTurnData(turnResult);
        _context.Players.ResetActions();
        _context.Players.UpdatePositions();
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