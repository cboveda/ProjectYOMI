using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControls : MonoBehaviour
{
    public static PlayerControls Instance { get; private set; }

    [SerializeField] private CharacterMoveDatabase _moveDatabase;

    [SerializeField] private MoveButton _lightAttackButton;
    [SerializeField] private MoveButton _heavyAttackButton;
    [SerializeField] private MoveButton _parryButton;
    [SerializeField] private MoveButton _grabButton;
    [SerializeField] private MoveButton _specialButton;
    [SerializeField] private GameObject _helperArrows;

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

    public MoveButton GetButtonByType(CharacterMove.Type type)
    {
        return type switch
        {
            CharacterMove.Type.LightAttack => _lightAttackButton,
            CharacterMove.Type.HeavyAttack => _heavyAttackButton,
            CharacterMove.Type.Parry => _parryButton,
            CharacterMove.Type.Grab => _grabButton,
            CharacterMove.Type.Special => _specialButton,
            _ => null,
        };
    }

    public void RegisterCharacterMoveSet(int lightId, int heavyId, int parryId, int grabId, int specialId)
    {
        _lightAttackButton.SetMove(_moveDatabase.GetMoveById(lightId));
        _heavyAttackButton.SetMove(_moveDatabase.GetMoveById(heavyId));
        _parryButton.SetMove(_moveDatabase.GetMoveById(parryId));
        _grabButton.SetMove(_moveDatabase.GetMoveById(grabId));
        _specialButton.SetMove(_moveDatabase.GetMoveById(specialId));
    }

    public void ToggleHelperArrows()
    {
        _helperArrows.SetActive(!_helperArrows.activeSelf);
    }
}
