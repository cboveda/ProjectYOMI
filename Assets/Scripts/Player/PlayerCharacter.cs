using Unity.Netcode;
using UnityEngine;

public class PlayerCharacter : NetworkBehaviour
{
    [SerializeField] private Character character;

    public NetworkVariable<int> health = new(100, NetworkVariableReadPermission.Everyone);
    public int Health { get { return health.Value; } set { health.Value = value; } }

    public NetworkVariable<int> SelectedMove = new(-1);

    private bool hasRegisteredWithUI = false;

    public void Update()
    {
        if (!IsLocalPlayer) return;
        if (hasRegisteredWithUI) return;
        if (PlayerControls.Instance == null) return;

        PlayerControls.Instance.RegisterCharacterById(character.Id);
        hasRegisteredWithUI = true;
    }

    [ServerRpc]
    public void SubmitPlayerActionServerRpc(int moveId)
    {
        SelectedMove.Value = moveId;
    }

    public void ResetSelectedMove()
    {
        SelectedMove.Value = -1;
    }


}
