using Unity.Netcode;
using UnityEngine;

public class PlayerCharacter : NetworkBehaviour
{
    [SerializeField] private Character Character;

    private bool registered = false;

    public override void OnNetworkSpawn()
    {
        /* This doesn't work */
        //if (!IsLocalPlayer) return;
        //PlayerControls.Instance.RegisterCharacterById(Character.Id);
    }

    public void Update()
    {
        if (!IsLocalPlayer) return;
        if (registered) return;
        if (PlayerControls.Instance == null) return;

        PlayerControls.Instance.RegisterCharacterById(Character.Id);
        registered = true;
    }
}
