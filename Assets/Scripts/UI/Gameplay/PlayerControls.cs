using Unity.Netcode;
using UnityEngine;

public class PlayerControls : NetworkBehaviour
{
    public static PlayerControls Instance { get; private set; }

    [SerializeField] MoveButton lightAttackButton;
    [SerializeField] MoveButton heavyAttackButton;
    [SerializeField] MoveButton parryButton;
    [SerializeField] MoveButton grabButton;
    [SerializeField] MoveButton specialButton;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    [ClientRpc]
    public void RegisterPlayerClientRpc(CharacterMoveSet moveSet, ClientRpcParams clientRpcParams = default)
    {
        if (!IsOwner) return;

        lightAttackButton.SetMove(moveSet.LightAttack);
        heavyAttackButton.SetMove(moveSet.HeavyAttack);
        parryButton.SetMove(moveSet.Parry);
        grabButton.SetMove(moveSet.Grab);
        specialButton.SetMove(moveSet.Special);
    }
}
