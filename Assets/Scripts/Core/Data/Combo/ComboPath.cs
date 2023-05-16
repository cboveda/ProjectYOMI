using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterComboPath", menuName = "Characters/Combo Path")]
public class ComboPath : ScriptableObject
{
    [SerializeField] private SerializableDictionary<Move.Type, Move.Type> _freshComboMoves = new();
    [SerializeField] private SerializableDictionary<Move.Type, Move.Type> _comboMoves = new();
    [SerializeField] private SerializableDictionary<Move.Type, Move.Type[]> _freshMixupMoves = new();
    [SerializeField] private SerializableDictionary<Move.Type, Move.Type[]> _mixupMoves = new();

}