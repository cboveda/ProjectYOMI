
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterMoveSet", menuName = "Characters/Character Move Set")]
public class CharacterMoveSet : ScriptableObject, INetworkSerializable
{
    [SerializeField] private CharacterMove _lightAttack;
    [SerializeField] private CharacterMove _heavyAttack;
    [SerializeField] private CharacterMove _parry;
    [SerializeField] private CharacterMove _grab;
    [SerializeField] private CharacterMove _special;

    public CharacterMove LightAttack => _lightAttack;
    public CharacterMove HeavyAttack => _heavyAttack;
    public CharacterMove Parry => _parry;
    public CharacterMove Grab => _grab;
    public CharacterMove Special => _special;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref _lightAttack);
        serializer.SerializeValue(ref _heavyAttack);
        serializer.SerializeValue(ref _parry);
        serializer.SerializeValue(ref _grab);
        serializer.SerializeValue(ref _special);
    }
}
