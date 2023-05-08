using UnityEngine;
using Unity.Netcode;

public class TurnHistory : NetworkBehaviour, ITurnHistory
{
    //History
    private NetworkList<TurnData> _turnDataList;
    public NetworkList<TurnData> TurnDataList { get { return _turnDataList; } }

    private void Awake()
    {
        _turnDataList = new NetworkList<TurnData>();
    }

    public void AddTurnData(TurnData turnData)
    {
        _turnDataList.Add(turnData);
    }

    public int GetCurrentTurnNumber()
    {
        return _turnDataList.Count + 1;
    }
}
