
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterMoveSet", menuName = "Characters/Character Move Set")]
public class CharacterMoveSet : ScriptableObject, INetworkSerializable
{
    [SerializeField] private CharacterMove lightAttack;
    [SerializeField] private CharacterMove heavyAttack;
    [SerializeField] private CharacterMove parry;
    [SerializeField] private CharacterMove grab;
    [SerializeField] private CharacterMove special;

    public CharacterMove LightAttack => lightAttack;
    public CharacterMove HeavyAttack => heavyAttack;
    public CharacterMove Parry => parry;
    public CharacterMove Grab => grab;
    public CharacterMove Special => special;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref lightAttack);
        serializer.SerializeValue(ref heavyAttack);
        serializer.SerializeValue(ref parry);
        serializer.SerializeValue(ref grab);
        serializer.SerializeValue(ref special);
    }
}
