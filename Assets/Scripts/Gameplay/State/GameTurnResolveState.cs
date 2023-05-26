using Castle.Core.Configuration;
using System.Linq;

public class GameTurnResolveState : GameBaseState
{
    public GameTurnResolveState(IGameStateMachine currentContext, GameStateFactory gameStateFactory) : base(currentContext, gameStateFactory)
    {
    }

    public override void CheckSwitchStates()
    {
        if (_context.TimerComplete)
        {
            if (GameShouldEnd())
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
        _context.SetTimer(_context.CombatConfiguration.RoundResolveDuration);
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
            .DetermineNextComboMove()
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


    private bool GameShouldEnd()
    {
        return _context.Players.GetAll().Any(pc => 
            pc.PlayerData.Health <= 0 || 
            pc.PlayerData.Position < _context.CombatConfiguration.PositionMinimumForRingOut);
    }
}