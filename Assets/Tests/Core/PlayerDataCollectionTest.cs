using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerDataCollectionTest
{
    PlayerDataCollection _collection;
    bool _initialized = false;

    NetworkManager _networkManager;
    PlayerCharacter[] _playerCharacters;

    [UnitySetUp]
    public IEnumerator OneTimeUnitySetUp()
    {
        if (!_initialized)
        {
            _initialized = true;
            var networkManagerPrefab = Resources.Load("Tests/NetworkManagerTester");
            var testObjectPrefab = (GameObject)Resources.Load("Tests/PlayerCharacterAndDataTester");
            var networkManagerObject = (GameObject)MonoBehaviour.Instantiate(networkManagerPrefab);
            _networkManager = networkManagerObject.GetComponent<NetworkManager>();
            _networkManager.StartHost();
            _networkManager.AddNetworkPrefab(testObjectPrefab);

            _playerCharacters = new PlayerCharacter[2];
            for (int i = 0; i < 2; i++)
            {
                var testObject1 = (GameObject)MonoBehaviour.Instantiate(testObjectPrefab);
                testObject1.GetComponent<NetworkObject>().Spawn();
                var playerCharacter = testObject1.GetComponent<PlayerCharacter>();
                playerCharacter.ClientId = (ulong) i;
                playerCharacter.PlayerNumber = i + 1;
                _playerCharacters[i] = playerCharacter;
            }
            _collection = new PlayerDataCollection();
            yield return null;
        }
    } 

    [Test, Order(1)]
    public void InitializesWithEmptyCollection()
    {
        Assert.AreEqual(0, _collection.GetAll().Length);
    }

    [Test, Order(2)]
    [TestCase(0)]
    [TestCase(1)]
    public void RegisteringPlayerCharacterAddsToCollectionAndSetsClientId(int index)
    {
        var playerCharacter = _playerCharacters[index];
        _collection.RegisterPlayerCharacter(
            playerNumber: playerCharacter.PlayerNumber,
            clientId: playerCharacter.ClientId,
            playerCharacter: playerCharacter);
        bool playerCharacterFoundInCollection = _collection.GetAll().Any<PlayerCharacter>(pc => pc.ClientId == playerCharacter.ClientId);
        ulong clientIdValueInCollection = (index == 0) ? _collection.ClientIdPlayer1 : _collection.ClientIdPlayer2;
        Assert.IsTrue(playerCharacterFoundInCollection);
        Assert.AreEqual(playerCharacter.ClientId, clientIdValueInCollection);
    }

    [Test]
    [TestCase(0)]
    [TestCase(1)]
    public void GetByClientIdReturnsCorrectPlayerCharacter(int index)
    {
        var compare = _playerCharacters[index];
        var output = _collection.GetByClientId(compare.ClientId);
        Assert.AreEqual(compare, output);
    }

    [Test]
    public void GetByClientIdThrowsExceptionForInvalidId()
    {
        Assert.Throws<System.Exception>(() => _collection.GetByClientId(ulong.MaxValue));
    }

    [Test]
    [TestCase(0)]
    [TestCase(1)]
    public void GetByPlayerNumberReturnsCorrectPlayerCharacter(int index)
    {
        var compare = _playerCharacters[index];
        var output = _collection.GetByPlayerNumber(compare.PlayerNumber);
        Assert.AreEqual(compare, output);
    }

    [Test]
    public void GetByPlayerNumberThrowsExceptionIfPlayerNotFound()
    {
        Assert.Throws<System.Exception>(() => _collection.GetByPlayerNumber(int.MaxValue));
    }

    [Test]
    [TestCase(0)]
    [TestCase(1)]
    public void GetByOpponentIdReturnsCorrectPlayerCharacter(int index)
    {
        var myPlayerCharacter = _playerCharacters[index];
        var other = _playerCharacters[(index == 0) ? 1 : 0];
        var output = _collection.GetByOpponentClientId(myPlayerCharacter.ClientId);
        Assert.AreEqual(other, output);
    }

    [Test]
    public void GetByOpponentIdThrowsExceptionForInvalidId()
    {
        Assert.Throws<System.Exception>(() => _collection.GetByOpponentClientId(ulong.MaxValue));
    }

    [Test]
    public void GameShouldEndReturnsFalseWhenBothPlayerHealthsAreNotZero()
    {
        Assert.IsFalse(_collection.GameShouldEnd());
    }

    [Test]
    [TestCase((ulong) 0)]
    [TestCase((ulong) 1)]
    public void GameShouldEndReturnsTrueWhenAPlayerHealthIsZero(ulong index)
    {
        _collection.GetByClientId(index).Health = 0;
        Assert.IsTrue(_collection.GameShouldEnd());
        _collection.GetByClientId(index).Health = 1;
    }
}
