using System.Collections;
using Moq;
using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.TestTools;

public class CharacterSelectDisplayTest
{
    NetworkManager _networkManager;
    GameObject _testObject;
    CharacterSelectDisplay _characterSelectDisplay;
    Mock<IServerManager> _serverManager;
    bool _initialized = false;

    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        if (!_initialized)
        {
            _initialized = true;
            var networkManagerPrefab = Resources.Load("Tests/NetworkManagerTester");
            var testObjectPrefab = (GameObject)Resources.Load("Tests/CharacterSelectDisplayTester");
            var networkManagerObject = (GameObject)MonoBehaviour.Instantiate(networkManagerPrefab);
            _networkManager = networkManagerObject.GetComponent<NetworkManager>();
            _networkManager.StartHost();
            _networkManager.AddNetworkPrefab(testObjectPrefab);
            _testObject = (GameObject)MonoBehaviour.Instantiate(testObjectPrefab);
            _characterSelectDisplay = _testObject.GetComponent<CharacterSelectDisplay>();

            _serverManager = new Mock<IServerManager>();
            _serverManager.Setup(m => m.JoinCode).Returns("JOIN");
            _serverManager.Setup(m => m.SetCharacter(It.IsAny<ulong>(), It.IsAny<int>())).Verifiable();
            _serverManager.Setup(m => m.StartGame()).Verifiable();
            _characterSelectDisplay.Construct(_networkManager, _serverManager.Object);

            _testObject.GetComponent<NetworkObject>().Spawn();
            yield return null;
        }
    }

    [Test]
    public void InitializesCorrectly()
    {
        Assert.IsTrue(_characterSelectDisplay.CharacterButtons.Count > 0);
        Assert.IsTrue(_characterSelectDisplay.Players.Count > 0);
        Assert.IsTrue(_characterSelectDisplay.PlayerCards[0].isActiveAndEnabled);
    }

    [Test]
    public void HandlesDisconnectCorrectly()
    {
        var clientId = _networkManager.LocalClientId;
        _characterSelectDisplay.HandleClientDisconnected(clientId);
        Assert.AreEqual(0, _characterSelectDisplay.Players.Count);
        _characterSelectDisplay.HandleClientConnected(clientId);
    }

    [Test]
    public void CanSelectCorrectly()
    {
        var character = (Character) _characterSelectDisplay.CharacterDatabase.GetAllCharacters()[0];
        _characterSelectDisplay.Select(character);
        Assert.IsTrue(_characterSelectDisplay.CharacterInfoPanel.activeSelf);
        Assert.IsNotNull(_characterSelectDisplay.IntroInstance);
        Assert.AreEqual(character.Id, _characterSelectDisplay.Players[0].CharacterId);
    }

    [Test]
    public void CanLockInCorrectly()
    {
        var character = (Character)_characterSelectDisplay.CharacterDatabase.GetAllCharacters()[0];
        _characterSelectDisplay.Select(character);
        _characterSelectDisplay.LockIn();
        Assert.IsTrue(_characterSelectDisplay.Players[0].IsLockedIn);
        _serverManager.Verify(m => m.SetCharacter(_networkManager.LocalClientId, character.Id), Times.Once());
        _serverManager.Verify(m => m.StartGame(), Times.Once());
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        _networkManager.GetComponent<NetworkManager>().Shutdown();
        GameObject.Destroy(_networkManager.gameObject);
        GameObject.Destroy(_testObject);
    }
}
