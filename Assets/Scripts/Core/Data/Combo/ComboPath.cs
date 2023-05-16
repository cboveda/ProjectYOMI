using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "NewCharacterComboPath", menuName = "Characters/Combo Path")]
public class ComboPath : ScriptableObject
{
    [SerializeField] private Move.Type _comboMove;
    [SerializeField] private Move.Type _freshComboMove;
    
    private IDatabase _database;

    [Inject]
    public void Construct(IDatabase database)
    {
        _database = database;
    }

    public Move.Type ComboMove { get => _comboMove; }
    public Move.Type FreshComboMove { get => _freshComboMove; }
    public Move.Type[] MixUp { get => GetMixUpsForComboMove(_comboMove); }
    public Move.Type[] FreshMixUp { get => GetMixUpsForComboMove(_freshComboMove); }

    private Move.Type[] GetMixUpsForComboMove(Move.Type comboMove)
    {
        if (_database == null)
        {
            Debug.LogError("Please add ComboPath scriptable object to the container for injection.");
            return null;
        }

        List<Move.Type> output = new();
        // Find the counters to the combo move (except special)
        var counters = _database.MoveInteractions.DefeatsType(comboMove)
            .Where(t => t != Move.Type.Special)
            .ToList();
        // Find the counters to the counters (except special)
        counters.ForEach(counter => output.AddRange(_database.MoveInteractions.DefeatsType(counter).Where(t => t != Move.Type.Special)));
        // Remove duplicates
        return new HashSet<Move.Type>(output).ToArray();
    }
}