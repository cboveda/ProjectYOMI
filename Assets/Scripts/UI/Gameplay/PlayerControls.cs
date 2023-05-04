using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PlayerControls : MonoBehaviour
{
    public static PlayerControls Instance { get; private set; }

    private Database _database;

    [SerializeField] private MoveButton _lightAttackButton;
    [SerializeField] private MoveButton _heavyAttackButton;
    [SerializeField] private MoveButton _parryButton;
    [SerializeField] private MoveButton _grabButton;
    [SerializeField] private MoveButton _specialButton;
    [SerializeField] private GameObject _helperArrows;

    [Inject]
    public void Construct(Database database)
    {
        _database = database;
    }

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

    public MoveButton GetButtonByType(Move.Type type)
    {
        return type switch
        {
            Move.Type.LightAttack => _lightAttackButton,
            Move.Type.HeavyAttack => _heavyAttackButton,
            Move.Type.Parry => _parryButton,
            Move.Type.Grab => _grabButton,
            Move.Type.Special => _specialButton,
            _ => null,
        };
    }

    public void RegisterCharacterMoveSet(int lightId, int heavyId, int parryId, int grabId, int specialId)
    {
        _lightAttackButton.SetMove(_database.Moves.GetMoveById(lightId));
        _heavyAttackButton.SetMove(_database.Moves.GetMoveById(heavyId));
        _parryButton.SetMove(_database.Moves.GetMoveById(parryId));
        _grabButton.SetMove(_database.Moves.GetMoveById(grabId));
        _specialButton.SetMove(_database.Moves.GetMoveById(specialId));
    }

    public void ToggleHelperArrows()
    {
        _helperArrows.SetActive(!_helperArrows.activeSelf);
    }
}
