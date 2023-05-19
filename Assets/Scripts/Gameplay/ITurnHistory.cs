using Unity.Netcode;

public interface ITurnHistory
{
    NetworkList<TurnResult> TurnDataList { get; }

    void AddTurnData(TurnResult turnData);
    int GetCurrentTurnNumber();
}