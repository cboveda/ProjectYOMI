using System.Linq;
using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(fileName = "NewCharacterMove", menuName = "Moves/Character Move")]
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
    [SerializeField] private bool _usableByDefault;
    [SerializeField] private int _id;

    public string MoveName { get => _moveName; set => _moveName = value; }
    public Type MoveType { get => _moveType; set => _moveType = value; }
    public bool UsableByDefault { get => _usableByDefault; set => _usableByDefault = value; }
    public int Id { get => _id; set => _id = value; }

    public override string ToString()
    {
        return $"{_moveName} [{_id}] ({_moveType})";
    }
}
