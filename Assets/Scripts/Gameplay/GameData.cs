using UnityEngine;
using Unity.Netcode;

public class GameData : NetworkBehaviour
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
}
