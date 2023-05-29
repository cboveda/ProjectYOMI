using Unity.Netcode;

public interface IGameStateMachine
{
    GameBaseState CurrentState { get; set; }
    bool TimerComplete { get; }
    IGameUIManager GameplayUI { get; }
    ITurnHistory TurnHistory { get; }
    IPlayerDataCollection Players { get; }
    CombatConfiguration CombatConfiguration { get; }
    IDatabase Database { get; }
    TurnFactory TurnFactory { get; }

    void SetTimer(float max);
}