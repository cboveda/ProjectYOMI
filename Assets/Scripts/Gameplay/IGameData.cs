using Unity.Netcode;

public interface IGameData
{
    NetworkList<TurnData> TurnDataList { get; }

    bool GameShouldEnd();
    void SubmitPlayerActionServerRpc(int moveId, ServerRpcParams serverRpcParams = default);
}