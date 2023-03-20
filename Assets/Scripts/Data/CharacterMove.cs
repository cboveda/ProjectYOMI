using System.Collections;
using System.Collections.Generic;
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

    public string MoveName => moveName;
    public Type MoveType => moveType;
    public Type[] DefeatsTypes => defeatsTypes;
    public bool UsableByDefault => usableByDefault;

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
}
