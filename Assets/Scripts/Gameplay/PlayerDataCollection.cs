using System.Collections.Generic;
using System.Linq;

public class PlayerDataCollection : IPlayerDataCollection
{
    private readonly Dictionary<ulong, IPlayerCharacter> _playerCharacters;
    private ulong _clientIdPlayer1;
    private ulong _clientIdPlayer2;
    public ulong ClientIdPlayer1 { get => _clientIdPlayer1; set => _clientIdPlayer1 = value; }
    public ulong ClientIdPlayer2 { get => _clientIdPlayer2; set => _clientIdPlayer2 = value; }

    public PlayerDataCollection()
    {
        _playerCharacters = new Dictionary<ulong, IPlayerCharacter>();
    }

    public void RegisterPlayerCharacter(int playerNumber, ulong clientId, IPlayerCharacter playerCharacter)
    {
        if (playerNumber == 1)
        {
            ClientIdPlayer1 = clientId;
        }
        else
        {
            ClientIdPlayer2 = clientId;
        }
        _playerCharacters.Add(clientId, playerCharacter);
    }

    public IPlayerCharacter[] GetAll()
    {
        return _playerCharacters.Values.ToArray();
    }

    public IPlayerCharacter GetByClientId(ulong clientId)
    {
        if (!_playerCharacters.TryGetValue(clientId, out IPlayerCharacter playerCharacter))
        {
            throw new System.Exception($"Player character for client: {clientId} not found.");
        }
        return playerCharacter;
    }

    public IPlayerCharacter GetByPlayerNumber(int playerNumber)
    {
        var clientId =
            playerNumber == 1 ? ClientIdPlayer1 :
            playerNumber == 2 ? ClientIdPlayer2 :
            ulong.MaxValue;
        if (!_playerCharacters.TryGetValue(clientId, out var playerCharacter))
        {
            throw new System.Exception($"Player character for player#: {playerNumber} not found.");
        }
        return playerCharacter;
    }

    public IPlayerCharacter GetByOpponentClientId(ulong clientId)
    {
        var targetId =
            ClientIdPlayer1 == clientId ? ClientIdPlayer2 :
            ClientIdPlayer2 == clientId ? ClientIdPlayer1 :
            ulong.MaxValue;
        if (!_playerCharacters.TryGetValue(targetId, out IPlayerCharacter playerCharacter))
        {
            throw new System.Exception($"Player character for opponent id: {clientId} not found.");
        }
        return playerCharacter;
    }


    public void UpdatePositions()
    {
        foreach (var pc in _playerCharacters.Values)
        {
            pc.PlayerMovementController.UpdatePosition();
        }
    }

    public void ResetActions()
    {
        foreach (var pc in _playerCharacters.Values)
        {
            pc.ResetAction();
        }
    }
}
