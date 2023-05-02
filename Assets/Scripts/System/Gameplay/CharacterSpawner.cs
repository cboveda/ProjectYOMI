using System;
using UnityEngine;
using Unity.Netcode;
using Zenject;

public class CharacterSpawner : NetworkBehaviour
{
    [SerializeField] private CharacterDatabase _characterDatabase;
    private IGameUIManager _gameUIManager;
    [SerializeField] private GameData _gameData;
    [SerializeField] private GameObject _player1SpawnLocation;
    [SerializeField] private GameObject _player2SpawnLocation;

    private bool _hasSpawnedPlayer1 = false;

    [Inject]
    public void Construct(IGameUIManager gameUIManager)
    {
        _gameUIManager = gameUIManager;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            return;
        }

        foreach (var client in ServerManager.Instance.ClientData)
        {
            var clientId = client.Value.clientId;
            var characterId = client.Value.characterId;
            var character = _characterDatabase.GetCharacterById(characterId);
            if (character == null)
            {
                throw new Exception($"Error getting character from database. characterId: {client.Value.characterId}");
            }
            SpawnPlayerObjectForGivenClientId(clientId, character, out var instance);
            if (!instance.TryGetComponent<PlayerCharacter>(out var playerCharacter))
            {
                throw new Exception("Error getting PlayerCharacter component of player object");
            }
            playerCharacter.GameUIManager = _gameUIManager;
            playerCharacter.ClientId = clientId;
            playerCharacter.PlayerNumber = (_hasSpawnedPlayer1) ? 2 : 1;
            RegisterPlayerObjectWithSystems(clientId, playerCharacter);

            _hasSpawnedPlayer1 = true;
        }
    }

    private void SpawnPlayerObjectForGivenClientId(ulong clientId, Character character, out NetworkObject instance)
    {
        var spawnPos = _hasSpawnedPlayer1 ? _player2SpawnLocation.transform : _player1SpawnLocation.transform;
        var characterInstance = Instantiate(character.GameplayPrefab, spawnPos);
        characterInstance.SpawnAsPlayerObject(clientId);
        instance = characterInstance;
    }

    private void RegisterPlayerObjectWithSystems(ulong clientId, PlayerCharacter playerCharacter)
    {
        _gameData.RegisterPlayerCharacter(playerCharacter.PlayerNumber, clientId, playerCharacter);
        _gameUIManager.RegisterPlayerCharacter(playerCharacter.PlayerNumber, clientId);
    }
}
