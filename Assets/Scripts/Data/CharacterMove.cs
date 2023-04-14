using System.Linq;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterMove", menuName = "Characters/Character Move")]
public class CharacterMove : ScriptableObject, INetworkSerializable
{
    [SerializeField] private string moveName;
    [SerializeField] private Type moveType;
    [SerializeField] private Type[] defeatsTypes;
    [SerializeField] private bool usableByDefault;
    [SerializeField] private int id;

    public string MoveName => moveName;
    public Type MoveType => moveType;
    public Type[] DefeatsTypes => defeatsTypes;
    public bool UsableByDefault => usableByDefault;
    public int Id => id;

    public bool Defeats(Type other)
    {
        return defeatsTypes.Any<Type>(t => t == other);
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref moveName);
        serializer.SerializeValue(ref moveType);
        serializer.SerializeValue(ref defeatsTypes);
        serializer.SerializeValue(ref usableByDefault);
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
        return $"{moveName} [{Id}] ({moveType})";
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
        return System.HashCode.Combine(base.GetHashCode(), name, id);
    }
}
