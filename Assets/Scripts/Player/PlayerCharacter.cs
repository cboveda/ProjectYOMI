using Unity.Netcode;
using UnityEngine;

public class PlayerCharacter : NetworkBehaviour
{
    [SerializeField] private Character character;

    private bool _hasRegistered = false;

    public void Update()
    {
        if (!IsLocalPlayer) return;
        if (_hasRegistered) return;
        if (PlayerControls.Instance == null) return;

        PlayerControls.Instance.RegisterCharacterById(character.Id);
        _hasRegistered = true;
    }
}
