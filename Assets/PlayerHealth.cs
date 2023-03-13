using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    public void SetHealth(int value)
    {
        ulong localClientId = NetworkManager.Singleton.LocalClientId;

        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(localClientId, out NetworkClient networkClient)) return;

        if (!networkClient.PlayerObject.TryGetComponent<Player>(out Player player)) return;

        //player.SetPlayerHealthServerRpc(100);
    }
}
