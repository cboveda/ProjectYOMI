
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Characters/Character")]
public class Character : ScriptableObject
{
    [SerializeField] private int _id = -1;
    [SerializeField] private string _displayName = "New Display Name";
    [SerializeField] private string _informationText;
    [SerializeField] private Sprite _icon;
    [SerializeField] private GameObject _introPrefab;
    [SerializeField] private NetworkObject _gameplayPrefab;
    [SerializeField] private CharacterMoveSet _characterMoveSet;
    [SerializeField] private ComboPathSet _comboPathSet;
    [SerializeField] private int _maximumHealth;
    [SerializeField] private CharacterBaseEffect _effect;

    public int Id => _id;
    public string DisplayName => _displayName;
    public string InformationText => _informationText;
    public Sprite Icon => _icon;
    public GameObject IntroPrefab => _introPrefab;
    public NetworkObject GameplayPrefab => _gameplayPrefab;
    public CharacterMoveSet CharacterMoveSet => _characterMoveSet;
    public int MaximumHealth => _maximumHealth;
    public CharacterBaseEffect Effect => _effect;
    public ComboPathSet ComboPathSet => _comboPathSet;
}
