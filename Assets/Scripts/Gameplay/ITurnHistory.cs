using Unity.Netcode;

public interface ITurnHistory
{
    NetworkList<TurnData> TurnDataList { get; }

    void AddTurnData(TurnData turnData);
    int GetCurrentTurnNumber();
}