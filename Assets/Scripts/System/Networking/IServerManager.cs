using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;

public interface IServerManager
{
    Dictionary<ulong, ClientData> ClientData { get; }
    string JoinCode { get; }

    void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response);
    IEnumerator DelayedSceneLoad();
    void Disconnect();
    void ResetGame();
    void SetCharacter(ulong clientId, int characterId);
    void StartGame();
    void StartHost();
}