using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.TestTools;

public class TurnHistoryTest
{
    NetworkManager _networkManager;
    TurnHistory _turnHistory;

    bool _initialized = false;

    [UnitySetUp]
    public IEnumerator OneTimeSetUp()
    {
        if (!_initialized)
        {
            _initialized = true;
            var networkManagerPrefab = Resources.Load("Tests/NetworkManagerTester");
            var networkManagerObject = (GameObject)MonoBehaviour.Instantiate(networkManagerPrefab);
            var turnHistoryPrefab = (GameObject)Resources.Load("Tests/TurnHistoryTester");
            _networkManager = networkManagerObject.GetComponent<NetworkManager>();
            _networkManager.StartHost();
            _networkManager.AddNetworkPrefab(turnHistoryPrefab);
            var turnHistoryObject = (GameObject)MonoBehaviour.Instantiate(turnHistoryPrefab);
            turnHistoryObject.GetComponent<NetworkObject>().Spawn();
            _turnHistory = turnHistoryObject.GetComponent<TurnHistory>();
            yield return null;
        }
    }

    [Test]
    public void AddTurnDataAddsToCollection()
    {
        var initialCollectionLength = _turnHistory.TurnDataList.Count;
        var turnData = new TurnData();
        _turnHistory.AddTurnData(turnData);
        Assert.AreEqual(initialCollectionLength + 1, _turnHistory.TurnDataList.Count);
    }

    [Test]
    public void CurrentTurnNumberReflectsCollectionSize()
    {
        var initialTurnNumber = _turnHistory.GetCurrentTurnNumber();
        var turnData = new TurnData();
        _turnHistory.AddTurnData(turnData);
        Assert.AreEqual(initialTurnNumber + 1, _turnHistory.GetCurrentTurnNumber());
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        _networkManager.Shutdown();
        GameObject.Destroy(_networkManager.gameObject);
        GameObject.Destroy(_turnHistory.gameObject);
    }
}
