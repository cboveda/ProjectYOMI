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
        string result = (GameData.Instance.HealthPlayer1.Value <= 0) ? "Player 2 wins!" : "Player 1 wins!";
        GameUIManager.Instance.DisplayGameResultClientRpc(result);
    }

    public override void ExitState()
    {
    }

    public override void UpdateState()
    {
    }
}