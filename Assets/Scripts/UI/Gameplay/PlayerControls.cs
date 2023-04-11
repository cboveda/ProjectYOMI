using Unity.Netcode;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public static PlayerControls Instance { get; private set; }

    [SerializeField] CharacterDatabase characterDatabase;
    [SerializeField] MoveButton lightAttackButton;
    [SerializeField] MoveButton heavyAttackButton;
    [SerializeField] MoveButton parryButton;
    [SerializeField] MoveButton grabButton;
    [SerializeField] MoveButton specialButton;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void RegisterCharacterById(int characterId)
    {
        var character = characterDatabase.GetCharacterById(characterId);
        var moveSet = character.CharacterMoveSet;
        lightAttackButton.SetMove(moveSet.LightAttack);
        heavyAttackButton.SetMove(moveSet.HeavyAttack);
        parryButton.SetMove(moveSet.Parry);
        grabButton.SetMove(moveSet.Grab);
        specialButton.SetMove(moveSet.Special);
    }
}
