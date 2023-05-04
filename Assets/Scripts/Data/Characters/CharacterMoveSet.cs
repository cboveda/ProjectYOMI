using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterMoveSet", menuName = "Characters/Character Move Set")]
public class CharacterMoveSet : ScriptableObject
{
    [SerializeField] private Move _lightAttack;
    [SerializeField] private Move _heavyAttack;
    [SerializeField] private Move _parry;
    [SerializeField] private Move _grab;
    [SerializeField] private Move _special;

    public Move LightAttack => _lightAttack;
    public Move HeavyAttack => _heavyAttack;
    public Move Parry => _parry;
    public Move Grab => _grab;
    public Move Special => _special;

    public Move GetMoveByType(Move.Type type)
    {
        return type switch
        {
            Move.Type.LightAttack => _lightAttack,
            Move.Type.HeavyAttack => _heavyAttack,
            Move.Type.Parry => _parry,
            Move.Type.Grab => _grab,
            Move.Type.Special => _special,
            _ => null,
        };
    }
}
