using Unity.Netcode;
using UnityEngine;

public class PlayerCharacter : NetworkBehaviour
{
    [SerializeField] private Character character;
    [SerializeField] private CharacterMoveDatabase moveDatabase;

    public NetworkVariable<int> health = new(100, NetworkVariableReadPermission.Everyone);
    public int Health { get { return health.Value; } set { health.Value = value; } }

    public NetworkVariable<int> PendingPlayerMoveId = new(-1);

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
        PendingPlayerMoveId.Value = moveId;
        Debug.Log(moveDatabase.GetMoveById(moveId));
    }
}
