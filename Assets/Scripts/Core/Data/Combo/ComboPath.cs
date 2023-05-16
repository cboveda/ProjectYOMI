using System.Collections;
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

        //Find the counters to the counter of the specified combo move.
        List<Move.Type> output = new();
        _database.MoveInteractions.DefeatsType(comboMove).ForEach(l => output.AddRange(_database.MoveInteractions.DefeatsType(l)));
        return new HashSet<Move.Type>(output).ToArray();
    }
}