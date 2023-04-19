using Unity.Netcode;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public static PlayerControls Instance { get; private set; }

    [SerializeField] private CharacterDatabase _characterDatabase;
    [SerializeField] private MoveButton _lightAttackButton;
    [SerializeField] private MoveButton _heavyAttackButton;
    [SerializeField] private MoveButton _parryButton;
    [SerializeField] private MoveButton _grabButton;
    [SerializeField] private MoveButton _specialButton;

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
        var character = _characterDatabase.GetCharacterById(characterId);
        var moveSet = character.CharacterMoveSet;
        _lightAttackButton.SetMove(moveSet.LightAttack);
        _heavyAttackButton.SetMove(moveSet.HeavyAttack);
        _parryButton.SetMove(moveSet.Parry);
        _grabButton.SetMove(moveSet.Grab);
        _specialButton.SetMove(moveSet.Special);
    }
}
