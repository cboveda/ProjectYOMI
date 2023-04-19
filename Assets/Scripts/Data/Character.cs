
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Characters/Character")]
public class Character : ScriptableObject
{
    [SerializeField] private int _id = -1;
    [SerializeField] private string _displayName = "New Display Name";
    [SerializeField] private Sprite _icon;
    [SerializeField] private GameObject _introPrefab;
    [SerializeField] private NetworkObject _gameplayPrefab;
    [SerializeField] private CharacterMoveSet _characterMoveSet;
    [SerializeField] private int _maximumHealth;


    public int Id => _id;
    public string DisplayName => _displayName;
    public Sprite Icon => _icon;
    public GameObject IntroPrefab => _introPrefab;
    public NetworkObject GameplayPrefab => _gameplayPrefab;
    public CharacterMoveSet CharacterMoveSet => _characterMoveSet;
    public int MaximumHealth => _maximumHealth;
}
