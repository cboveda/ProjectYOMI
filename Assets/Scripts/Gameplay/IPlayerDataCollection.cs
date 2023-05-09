public interface IPlayerDataCollection
{
    ulong ClientIdPlayer1 { get; set; }
    ulong ClientIdPlayer2 { get; set; }

    bool GameShouldEnd();
    PlayerCharacter[] GetAll();
    PlayerCharacter GetByClientId(ulong clientId);
    PlayerCharacter GetByOpponentClientId(ulong clientId);
    PlayerCharacter GetByPlayerNumber(int playerNumber);
    void RegisterPlayerCharacter(int playerNumber, ulong clientId, PlayerCharacter playerCharacter);
}