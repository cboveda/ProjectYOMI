using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    //Todo: Use a network sync variable to control player color

    public override void OnNetworkSpawn()
    {
        if (NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
            Material material = playerObject.GetComponent<Renderer>().material;
            material.color = Color.blue;
        }
    }
}
