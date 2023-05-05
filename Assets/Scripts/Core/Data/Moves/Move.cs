using System.Linq;
using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(fileName = "NewCharacterMove", menuName = "Characters/Character Move")]
public class Move : ScriptableObject
{
    public static readonly int NO_MOVE = -1;

    public enum Type : byte
    {
        LightAttack = 16,
        HeavyAttack = 8,
        Parry = 4,
        Grab = 2,
        Special = 1
    }

    [SerializeField] private string _moveName;
    [SerializeField] private Type _moveType;
    [SerializeField] private Type[] _defeatsTypes;
    [SerializeField] private bool _usableByDefault;
    [SerializeField] private int _id;

    public string MoveName => _moveName;
    public Type MoveType => _moveType;
    public Type[] DefeatsTypes => _defeatsTypes;
    public bool UsableByDefault => _usableByDefault;
    public int Id => _id;

    public bool Defeats(Type other)
    {
        return _defeatsTypes.Any<Type>(t => t == other);
    }

    public override string ToString()
    {
        return $"{_moveName} [{_id}] ({_moveType})";
    }
}
