using System;
using UnityEngine;
using Unity.Netcode;
using Zenject;

public class CharacterSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject _player1SpawnLocation;
    [SerializeField] private GameObject _player2SpawnLocation;

    private bool _hasSpawnedPlayer1 = false;
    private Database _database;
    private IGameUIManager _gameUIManager;
    private IServerManager _serverManager;
    private PlayerDataCollection _players;
    private CombatEvaluator _combatEvaluator;
    private TurnHistory _turnHistory;

    [Inject]
    public void Construct(IGameUIManager gameUIManager, IServerManager serverManager, Database database, PlayerDataCollection players, CombatEvaluator combatEvaluator, TurnHistory turnHistory)
    {
        _gameUIManager = gameUIManager;
        _serverManager = serverManager;
        _database = database;
        _players = players;
        _combatEvaluator = combatEvaluator;
        _turnHistory = turnHistory;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            return;
        }

        foreach (var client in _serverManager.ClientData)
        {
            var clientId = client.Value.clientId;
            var characterId = client.Value.characterId;
            var character = _database.Characters.GetCharacterById(characterId);
            SpawnPlayerObjectForGivenClientId(clientId, character, out var instance);
            var playerCharacter = instance.GetComponent<PlayerCharacter>();
            InitializePlayerCharacterFields(clientId, playerCharacter);
            RegisterPlayerObjectWithSystems(clientId, playerCharacter);
            _hasSpawnedPlayer1 = true;
        }
    }

    private void InitializePlayerCharacterFields(ulong clientId, PlayerCharacter playerCharacter)
    {
        playerCharacter.GameUIManager = _gameUIManager;
        playerCharacter.ClientId = clientId;
        playerCharacter.PlayerNumber = (_hasSpawnedPlayer1) ? 2 : 1;
        playerCharacter.Effect.Contstruct(playerCharacter, _turnHistory, _players, _database, _combatEvaluator);
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
        _players.RegisterPlayerCharacter(playerCharacter.PlayerNumber, clientId, playerCharacter);
        _gameUIManager.RegisterPlayerCharacter(playerCharacter.PlayerNumber, clientId);
    }
}
