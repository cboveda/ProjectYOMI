using System.Collections.Generic;
using System.Linq;

public class PlayerDataCollection
{
    private readonly Dictionary<ulong, PlayerCharacter> _playerCharacters;
    private ulong _clientIdPlayer1;
    private ulong _clientIdPlayer2;
    public ulong ClientIdPlayer1 { get => _clientIdPlayer1; set => _clientIdPlayer1 = value; }
    public ulong ClientIdPlayer2 { get => _clientIdPlayer2; set => _clientIdPlayer2 = value; }

    public PlayerDataCollection()
    {
        _playerCharacters = new Dictionary<ulong, PlayerCharacter>();   
    }

    public void RegisterPlayerCharacter(int playerNumber, ulong clientId, PlayerCharacter playerCharacter)
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

    public PlayerCharacter[] GetAll()
    { 
        return _playerCharacters.Values.ToArray();
    }

    public PlayerCharacter GetByClientId(ulong clientId)
    {
        if (!_playerCharacters.TryGetValue(clientId, out PlayerCharacter playerCharacter))
        {
            throw new System.Exception($"Player character for client: {clientId} not found.");
        }
        return playerCharacter;
    }

    public PlayerCharacter GetByPlayerNumber(int playerNumber)
    {
        var clientId = playerNumber == 1 ? ClientIdPlayer1 : ClientIdPlayer2;
        if (!_playerCharacters.TryGetValue(clientId, out var playerCharacter))
        {
            return null;
        }
        return playerCharacter;
    }

    public PlayerCharacter GetByOpponentClientId(ulong clientId)
    {
        var targetId = (ClientIdPlayer1 == clientId) ? ClientIdPlayer2 : ClientIdPlayer1;

        if (!_playerCharacters.TryGetValue(targetId, out PlayerCharacter playerCharacter))
        {
            return null;
        }
        return playerCharacter;
    }

    public bool GameShouldEnd()
    {
        return _playerCharacters.Values.Any(pc => pc.PlayerData.Health <= 0);
    }
}
