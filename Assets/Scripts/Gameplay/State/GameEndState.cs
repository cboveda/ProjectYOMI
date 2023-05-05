using Unity.Netcode;

public class GameEndState : GameBaseState
{
    public GameEndState(GameStateMachine currentContext, GameStateFactory gameStateFactory) : base(currentContext, gameStateFactory)
    {
    }

    public override void CheckSwitchStates()
    {
    }

    public override void EnterState()
    {
        string result = "Game Over!";
        _context.GameplayUI.DisplayGameResultClientRpc(result);
    }

    public override void ExitState()
    {
    }

    public override void UpdateState()
    {
    }
}