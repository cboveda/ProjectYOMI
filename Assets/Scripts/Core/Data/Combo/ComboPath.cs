using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterComboPath", menuName = "Characters/Combo Path")]
public class ComboPath : ScriptableObject
{
    [SerializeField] private Move.Type _comboMove;
    [SerializeField] private Move.Type _freshComboMove;
    [SerializeField] private Move.Type[] _mixUp;
    [SerializeField] private Move.Type[] _freshMixUp;

    public Move.Type ComboMove { get => _comboMove; }
    public Move.Type FreshComboMove { get => _freshComboMove; }
    public Move.Type[] MixUp { get => _mixUp; }
    public Move.Type[] FreshMixUp { get => _freshMixUp; }
}