using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character Database", menuName = "Characters/Database")]
public class CharacterDatabase : ScriptableObject
{
    [SerializeField] private Character[] _characters = new Character[0];

    public Character[] GetAllCharacters () => _characters;
    public Character GetCharacterById(int id)
    {

        foreach(var character in _characters)
        {
            if (character.Id == id) return character;
        }
        return null;
    }

    public bool IsValidCharacterId(int id)
    {
        return _characters.Any(x => x.Id == id);
    }
}
