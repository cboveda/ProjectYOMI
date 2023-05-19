using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.TestTools;

public class CharacterSpawnerTest
{
    bool _initialized = false;
    NetworkManager _networkManager;
    CharacterSpawner _characterSpawner;
    Mock<IPlayerDataCollection> _players;
    Mock<IGameUIManager> _gameUIManager;

    [UnitySetUp]
    public IEnumerator OneTimeSetUp()
    {
        if (!_initialized)
        {
            _initialized = true;
            // Load resources and instantiate prefabs
            var networkManagerPrefab = Resources.Load("Tests/NetworkManagerTester");
            var characterSpawnerPrefab = (GameObject) Resources.Load("Tests/CharacterSpawnerTest");
            var character = (Character)Resources.Load("Tests/101_TestCharacter");
            
            var networkManagerObject = (GameObject) MonoBehaviour.Instantiate(networkManagerPrefab);
            _networkManager = networkManagerObject.GetComponent<NetworkManager>();
            _networkManager.StartHost();
            _networkManager.AddNetworkPrefab(characterSpawnerPrefab);

            var characterSpawnerObject = (GameObject)MonoBehaviour.Instantiate(characterSpawnerPrefab);
            _characterSpawner = characterSpawnerObject.GetComponent<CharacterSpawner>();

            // Create Mocks
            _gameUIManager = new Mock<IGameUIManager>();
            _players = new Mock<IPlayerDataCollection>();
            var database = new Mock<IDatabase>();
            var serverManager = new Mock<IServerManager>();
            var combatEvaluator = new Mock<CombatCommandExecutor>();
            var turnHistory = new Mock<ITurnHistory>();
            var cameraFocusObject = new Mock<ICameraFocusObject>();  

            // Setup Mocks
            _players
                .Setup(m => m.RegisterPlayerCharacter(
                    It.IsAny<int>(),
                    It.IsAny<ulong>(),
                    It.IsAny<PlayerCharacter>()))
                .Verifiable();

            _gameUIManager
                .Setup(m => m.RegisterPlayerCharacter(
                    It.IsAny<int>(),
                    It.IsAny<ulong>()))
                .Verifiable();

            database.Setup(m => m.Characters.GetCharacterById(It.IsAny<int>())).Returns(character);

            Dictionary<ulong, ClientData> data = new()
            {
                { 0, new ClientData(0) { characterId = 101 } },
            };
            serverManager.Setup(m => m.ClientData).Returns(data);

            // Install Mocks
            _characterSpawner.Construct(
                gameUIManager: _gameUIManager.Object,
                serverManager: serverManager.Object,
                database: database.Object,
                players: _players.Object,
                combatEvaluator: combatEvaluator.Object,
                turnHistory: turnHistory.Object,
                cameraFocusObject: cameraFocusObject.Object);

            _characterSpawner.GetComponent<NetworkObject>().Spawn();
            yield return null;
        }
    }

    [Test]
    public void SpawnsCharactersAndRegistersWithSystems()
    {
        var spawnedPlayers = GameObject.FindObjectsOfType<PlayerCharacter>();
        Assert.AreEqual(1, spawnedPlayers.Length);
        _players.Verify(
            m => m.RegisterPlayerCharacter(
                It.IsAny<int>(),
                It.IsAny<ulong>(),
                It.IsAny<PlayerCharacter>()),
            Times.Once);
        _gameUIManager.Verify(
            m => m.RegisterPlayerCharacter(
                It.IsAny<int>(),
                It.IsAny<ulong>()),
            Times.Once);
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        _networkManager.Shutdown();
        GameObject.Destroy(_networkManager.gameObject);
        GameObject.Destroy(_characterSpawner.gameObject);
        var allSpawnedObjects = GameObject.FindObjectsOfType<PlayerCharacter>();
        for (int i = 0; i < allSpawnedObjects.Length; i++)
        {
            GameObject.Destroy(allSpawnedObjects[i]);
        }
    }
}
