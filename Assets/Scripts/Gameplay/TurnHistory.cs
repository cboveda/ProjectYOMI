using UnityEngine;
using Unity.Netcode;

public class TurnHistory : NetworkBehaviour, ITurnHistory
{
    //History
    private NetworkList<TurnResult> _turnDataList;
    public NetworkList<TurnResult> TurnDataList { get { return _turnDataList; } }

    private void Awake()
    {
        _turnDataList = new NetworkList<TurnResult>();
    }

    public void AddTurnData(TurnResult turnData)
    {
        _turnDataList.Add(turnData);
    }

    public int GetCurrentTurnNumber()
    {
        return _turnDataList.Count + 1;
    }

    public bool GetLastTurn(out TurnResult lastTurn)
    {
        if (_turnDataList.Count == 0)
        {
            lastTurn = new TurnResult();
            return false;
        }
        lastTurn = _turnDataList[^1];
        return true;
    }
}
