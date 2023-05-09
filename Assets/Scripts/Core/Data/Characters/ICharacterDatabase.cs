public interface ICharacterDatabase
{
    Character[] GetAllCharacters();
    Character GetCharacterById(int id);
    bool IsValidCharacterId(int id);
}