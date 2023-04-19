using System.Linq;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterMove", menuName = "Characters/Character Move")]
public class CharacterMove : ScriptableObject, INetworkSerializable
{
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

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref _moveName);
        serializer.SerializeValue(ref _moveType);
        serializer.SerializeValue(ref _defeatsTypes);
        serializer.SerializeValue(ref _usableByDefault);
    }

    public enum Type : byte
    {
        LightAttack,
        HeavyAttack,
        Parry,
        Grab,
        Special
    }

    public override string ToString()
    {
        return $"{_moveName} [{Id}] ({_moveType})";
    }

    public override bool Equals(object other)
    {
        if (other == null)
        {
            return false;
        }
        if (other is not CharacterMove)
        {
            return false;
        }
        return this.Id == ((CharacterMove) other).Id;
    }

    public override int GetHashCode()
    {
        return System.HashCode.Combine(base.GetHashCode(), name, _id);
    }
}
