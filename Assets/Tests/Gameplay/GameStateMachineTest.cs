using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.TestTools;

public class GameStateMachineTest
{
    NetworkManager _networkManager;
    GameObject _testObject;
    GameStateMachine _gameStateMachine;
    bool _initialized = false;
    Mock<IGameUIManager> _gameUIManagerMock;
    Mock<ITurnHistory> _turnHistoryMock;
    Mock<PlayerDataCollection> _playerDataCollectionMock;
    Mock<CombatEvaluator> _combatEvaluatorMock;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        if (!_initialized)
        {
            _initialized = true;
            var networkManagerPrefab = Resources.Load("Tests/NetworkManagerTester");
            var testObjectPrefab = (GameObject)Resources.Load("Tests/GameStateMachineTester");
            var networkManagerObject = (GameObject)MonoBehaviour.Instantiate(networkManagerPrefab);
            _networkManager = networkManagerObject.GetComponent<NetworkManager>();
            _networkManager.StartHost();
            _networkManager.AddNetworkPrefab(testObjectPrefab);
            _testObject = (GameObject)MonoBehaviour.Instantiate(testObjectPrefab);
            _testObject.GetComponent<NetworkObject>().Spawn();
            _gameStateMachine = _testObject.GetComponent<GameStateMachine>();
            _gameUIManagerMock = new Mock<IGameUIManager>();
            _turnHistoryMock = new Mock<ITurnHistory>();
            _playerDataCollectionMock = new Mock<PlayerDataCollection>();
            _combatEvaluatorMock = new Mock<CombatEvaluator>();
            _gameStateMachine.Construct(
                networkManager: _networkManager,
                gameUIManager: _gameUIManagerMock.Object,
                turnHistory: _turnHistoryMock.Object,
                players: _playerDataCollectionMock.Object,
                combatEvaluator: _combatEvaluatorMock.Object);
            yield return null;
        }
    }

    [Test, Order(1)]
    public void GameStateMachineInitializesCorrectly()
    {
        Assert.AreEqual(typeof(GameStartState), _gameStateMachine.CurrentState.GetType());
    }

    [UnityTest, Order(2)]
    public IEnumerator TimeBasedStateChangesAfterTimerCompletes()
    {
        float testValue = 0.01f;
        var initialState = _gameStateMachine.CurrentState;
        _gameStateMachine.SetTimer(testValue);
        yield return new WaitForSeconds(testValue);
        Assert.AreNotSame(initialState, _gameStateMachine.CurrentState);
    }

    [Test]
    public void DependencyGettersReturnCorrectObject()
    {
        Assert.AreEqual(_combatEvaluatorMock.Object, _gameStateMachine.CombatEvaluator);
        Assert.AreEqual(_gameUIManagerMock.Object, _gameStateMachine.GameplayUI);
        Assert.AreEqual(_playerDataCollectionMock.Object, _gameStateMachine.Players);
        Assert.AreEqual(_turnHistoryMock.Object, _gameStateMachine.TurnHistory);
    }

    [Test]
    public void DurationAccessorsSetAndReturnCorrectValues()
    {
        float testValue = 1.123f;
        _gameStateMachine.GameStartDuration = testValue;
        _gameStateMachine.RoundActiveDuration = testValue;
        _gameStateMachine.RoundResolveDuration = testValue;
        Assert.AreEqual(testValue, _gameStateMachine.GameStartDuration);
        Assert.AreEqual(testValue, _gameStateMachine.RoundActiveDuration);
        Assert.AreEqual(testValue, _gameStateMachine.RoundResolveDuration);
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        _networkManager.GetComponent<NetworkManager>().Shutdown();
        GameObject.Destroy(_networkManager.gameObject);
        GameObject.Destroy(_testObject.gameObject);
    }
}




