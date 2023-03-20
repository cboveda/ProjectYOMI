using Unity.Netcode;

public class PlayerCharacter : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

    }
}
