using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMoveInteractions", menuName = "Moves/Move Interactions")]
public class MoveInteractions : ScriptableObject
{
    [SerializeField] private Move.Type[] _grabDefeatsTypes;
    [SerializeField] private Move.Type[] _heavyDefeatsTypes;
    [SerializeField] private Move.Type[] _lightDefeatsTypes;
    [SerializeField] private Move.Type[] _parryDefeatsTypes;
    [SerializeField] private Move.Type[] _specialDefeatsTypes;
    private Dictionary<Move.Type, List<Move.Type>> _defeatedByType;
    private Dictionary<Move.Type, List<Move.Type>> _defeatsType;
    public Dictionary<Move.Type, List<Move.Type>> DefeatedByType { get => _defeatedByType; }
    public Dictionary<Move.Type, List<Move.Type>> DefeatsType { get => _defeatsType; }

#if UNITY_INCLUDE_TESTS
    public Move.Type[] GrabDefeatsTypes { get => _grabDefeatsTypes; set => _grabDefeatsTypes = value; }
    public Move.Type[] HeavyDefeatsTypes { get => _heavyDefeatsTypes; set => _heavyDefeatsTypes = value; }
    public Move.Type[] LightDefeatsTypes { get => _lightDefeatsTypes; set => _lightDefeatsTypes = value; }
    public Move.Type[] ParryDefeatsTypes { get => _parryDefeatsTypes; set => _parryDefeatsTypes = value; }
    public Move.Type[] SpecialDefeatsTypes { get => _specialDefeatsTypes; set => _specialDefeatsTypes = value; }
#endif

    private void Awake()
    {
        InitializeDefeatedByTypeDictionary();
        InitializeDefeatsTypeDictionary();
        foreach (Move.Type type in Enum.GetValues(typeof(Move.Type)))
        {
            AddInteractionsForTypeToDictionaries(type);
        }
    }

    private void AddInteractionsForTypeToDictionaries(Move.Type targetType)
    {
        var typeArray = GetTypeArrayFor(targetType);
        foreach (var typeFromArray in typeArray)
        {
            if (_defeatedByType.TryGetValue(targetType, out var defeatedByList))
            {
                defeatedByList.Add(typeFromArray);
            }
            if (_defeatsType.TryGetValue(typeFromArray, out var defeatsList))
            {
                defeatsList.Add(targetType);
            }
        }
    }

    private Move.Type[] GetTypeArrayFor(Move.Type target)
    {
        return target switch
        {
            Move.Type.LightAttack => _lightDefeatsTypes,
            Move.Type.HeavyAttack => _heavyDefeatsTypes,
            Move.Type.Grab => _grabDefeatsTypes,
            Move.Type.Parry => _parryDefeatsTypes,
            Move.Type.Special => _specialDefeatsTypes,
            _ => null
        };
    }

    private void InitializeDefeatsTypeDictionary()
    {
        _defeatsType = new()
         {
            { Move.Type.LightAttack, new List<Move.Type>() },
            { Move.Type.HeavyAttack, new List<Move.Type>() },
            { Move.Type.Grab, new List<Move.Type>() },
            { Move.Type.Parry, new List<Move.Type>() },
            { Move.Type.Special, new List<Move.Type>() },
        };
    }

    private void InitializeDefeatedByTypeDictionary()
    {
        _defeatedByType = new()
        {
            { Move.Type.LightAttack, new List<Move.Type>() },
            { Move.Type.HeavyAttack, new List<Move.Type>() },
            { Move.Type.Grab, new List<Move.Type>() },
            { Move.Type.Parry, new List<Move.Type>() },
            { Move.Type.Special, new List<Move.Type>() },
        };
    }
}
