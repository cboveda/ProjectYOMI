using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMoveInteractions", menuName = "Moves/Move Interactions")]
public class MoveInteractions : ScriptableObject, IMoveInteractions
{
    [SerializeField] private MoveInteractionDictionary _defeatedByType;
    [SerializeField] private MoveInteractionDictionary _defeatsType;

    public List<Move.Type> DefeatedByType(Move.Type type) 
    {
        if (_defeatedByType.TryGetValue(type, out var list))
        {
            return list;
        }
        return null;
    }

    public List<Move.Type> DefeatsType(Move.Type type)
    {
        if (_defeatsType.TryGetValue(type, out var list))
        {
            return list;
        }
        return null;
    }
}
