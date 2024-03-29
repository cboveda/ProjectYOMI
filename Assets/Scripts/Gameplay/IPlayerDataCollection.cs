﻿public interface IPlayerDataCollection
{
    ulong ClientIdPlayer1 { get; set; }
    ulong ClientIdPlayer2 { get; set; }

    IPlayerCharacter[] GetAll();
    IPlayerCharacter GetByClientId(ulong clientId);
    IPlayerCharacter GetByOpponentClientId(ulong clientId);
    IPlayerCharacter GetByPlayerNumber(int playerNumber);
    void RegisterPlayerCharacter(int playerNumber, ulong clientId, IPlayerCharacter playerCharacter);
    void ResetActions();
    void UpdatePositions();
}