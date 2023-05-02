using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Unity.Netcode;
using System;

public class PlayerCharacterAndDataTest
{
    NetworkManager _networkManager;
    GameObject _testObject;
    PlayerCharacter _playerCharacter;
    bool _initialized = false;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        if (!_initialized)
        {
            _initialized = true;
            var networkManagerPrefab = Resources.Load("TestPrefabs/NetworkManagerTester");
            var testObjectPrefab = (GameObject)Resources.Load("TestPrefabs/PlayerCharacterAndDataTester");
            var networkManagerObject = (GameObject)MonoBehaviour.Instantiate(networkManagerPrefab);
            _networkManager = networkManagerObject.GetComponent<NetworkManager>();
            _networkManager.StartHost();
            _networkManager.AddNetworkPrefab(testObjectPrefab);
            _testObject = (GameObject)MonoBehaviour.Instantiate(testObjectPrefab);
            _testObject.GetComponent<NetworkObject>().Spawn();
            _playerCharacter = _testObject.GetComponent<PlayerCharacter>();
            _playerCharacter.ClientId = 0;
            _playerCharacter.PlayerNumber = 0;
            yield return null;
        }
    }

    [Test, Order(1)]
    public void IntializesCorrectly()
    {
        var playerData = _playerCharacter.PlayerData;
        var character = _playerCharacter.Character;
        var compare = new PlayerData(health: character.MaximumHealth);
        var usableMoveSet = _playerCharacter.UsableMoveSet;
        var characterEffect = _playerCharacter.Effect;
        Assert.AreEqual(playerData, compare);
        Assert.NotNull(usableMoveSet);
        Assert.NotNull(characterEffect);
    }

    [Test]
    [TestCase(-1, 0)]
    [TestCase(0, 0)]
    [TestCase(1, 1)]
    [TestCase(99, 99)]
    [TestCase(101, 100)]
    public void SetHealthCorrectly(float value, float expected)
    {
        _playerCharacter.Health = value;
        Assert.AreEqual(_playerCharacter.Health, expected);
    }

    [Test]
    [TestCase(-1, 0)]
    [TestCase(0, 0)]
    [TestCase(1, 1)]
    [TestCase(99, 99)]
    [TestCase(101, 100)]
    public void SetSpecialMeterCorrectly(float value, float expected)
    {
        _playerCharacter.SpecialMeter = value;
        Assert.AreEqual(_playerCharacter.SpecialMeter, expected);
    }




    [OneTimeTearDown]
    public void TearDown()
    {
        _networkManager.GetComponent<NetworkManager>().Shutdown();
        GameObject.Destroy(_networkManager.gameObject);
        GameObject.Destroy(_testObject);
    }
}
