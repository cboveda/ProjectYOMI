using Moq;
using NUnit.Framework;
using System.Collections;
using System.Linq;
using Unity.Netcode;
using UnityEngine.TestTools;
using UnityEngine;

public class PlayerCharacterAndDataTest
{
    bool _initialized = false;
    GameObject _testObject;
    Mock<IGameUIManager> _gameUIManagerMock;
    NetworkManager _networkManager;
    PlayerCharacter _playerCharacter;

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
            _testObject = (GameObject)MonoBehaviour.Instantiate(testObjectPrefab);
            _testObject.GetComponent<NetworkObject>().Spawn();
            _playerCharacter = _testObject.GetComponent<PlayerCharacter>();
            _playerCharacter.ClientId = 0;
            _playerCharacter.PlayerNumber = 0;
            yield return null;
        }
    }

    [SetUp]
    public void SetUp()
    {
        _gameUIManagerMock = new Mock<IGameUIManager>();
        _gameUIManagerMock
            .Setup(
                m => m.UpdateActiveSelectionButtonClientRpc(
                    It.IsAny<int>(), 
                    It.IsAny<int>(), 
                    It.Is<ClientRpcParams>(clientRpcParam => HasCorrectTargetClientId(
                        clientRpcParam, 
                        _playerCharacter.ClientId))))
            .Verifiable();
        _playerCharacter.GameUIManager = _gameUIManagerMock.Object;
    }

    [Test, Order(1)]
    public void PlayerCharacterClassIntializesCorrectly()
    {
        var playerData = _playerCharacter.PlayerData;
        var character = _playerCharacter.Character;
        var playerDataCompare = new PlayerData(health: character.MaximumHealth);
        var usableMoveSet = _playerCharacter.UsableMoveSet;
        var movementController = _playerCharacter.PlayerMovementController;
        Assert.AreEqual(playerDataCompare, playerData);
        Assert.NotNull(usableMoveSet);
        Assert.NotNull(movementController);
        Assert.NotNull(character.GameplayPrefab);
    }

    [Test]
    [TestCase(-1, 0)]
    [TestCase(0, 0)]
    [TestCase(1, 1)]
    [TestCase(99, 99)]
    [TestCase(101, 100)]
    public void HealthSetterChangesValueCorrectly(float value, float expected)
    {
        _playerCharacter.Health = value;
        Assert.AreEqual(expected, _playerCharacter.Health);
    }

    [Test]
    [TestCase(-1, 0)]
    [TestCase(0, 0)]
    [TestCase(1, 1)]
    [TestCase(99, 99)]
    [TestCase(101, 100)]
    public void SpecialMeterSetterChangesValueCorrectly(float value, float expected)
    {
        _playerCharacter.SpecialMeter = value;
        Assert.AreEqual(expected, _playerCharacter.SpecialMeter);
    }

    [Test]
    [TestCase(-1)]
    [TestCase(1)]
    [TestCase(2)]
    public void ActionSetterChangesValueCorrectly(int value)
    {
        _playerCharacter.Action = value;
        Assert.AreEqual(value, _playerCharacter.Action);
    }

    [Test]
    [TestCase(-1, -1)]
    [TestCase(0, 0)]
    [TestCase(1, 1)]
    public void PositionSetterChangesValueCorrectly(int value, int expected)
    {
        _playerCharacter.Position = value;
        Assert.AreEqual(expected, _playerCharacter.Position);
    }

    [Test]
    [TestCase(1)]
    [TestCase(2)]
    public void SubmitPlayerActionServerRpcSetsActionAndSendsClientRpc(int value)
    {
        _playerCharacter.SubmitPlayerActionServerRpc(value);
        Assert.AreEqual(value, _playerCharacter.Action);
        _gameUIManagerMock.Verify(
            m => m.UpdateActiveSelectionButtonClientRpc(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<ClientRpcParams>()),
            Times.Once());
    }

    [Test]
    [TestCase(-1, 0)]
    [TestCase(0, 0)]
    [TestCase(1, 1)]
    [TestCase(2, 2)]
    public void ComboCountSetterChangesValueCorrectly(int value, int expected)
    {
        _playerCharacter.ComboCount = value;
        Assert.AreEqual(expected, _playerCharacter.ComboCount);
    }

    [Test]
    public void ResetActionSetsValueAndSendsRpc()
    {
        _playerCharacter.ResetAction();
        Assert.AreEqual(_playerCharacter.Action, Move.NO_MOVE);
        _gameUIManagerMock.Verify(
            m => m.UpdateActiveSelectionButtonClientRpc(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<ClientRpcParams>()),
            Times.Once());
    }

    [Test]
    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(1)]
    public void DecreaseHealthChangesValueCorrectly(float value)
    {
        _playerCharacter.Health = _playerCharacter.Character.MaximumHealth / 2;
        float initial = _playerCharacter.Health;
        float expected = initial - value;
        _playerCharacter.DecreaseHealth(value);
        Assert.AreEqual(expected, _playerCharacter.Health);
    }

    [Test]
    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(1)]
    public void IncreaseSpecialMeterChangesValueCorrectly(float value)
    {
        _playerCharacter.SpecialMeter = 10;
        float initial = _playerCharacter.SpecialMeter;
        float expected = initial + value;
        _playerCharacter.IncreaseSpecialMeter(value);
        Assert.AreEqual(expected, _playerCharacter.SpecialMeter);
    }

    [Test]
    public void IncrementComboCountChangesValueCorrectly()
    {
        int initial = _playerCharacter.ComboCount;
        int expected = initial + 1;
        _playerCharacter.IncrementComboCount();
        Assert.AreEqual(expected, _playerCharacter.ComboCount);
    }

    [Test]
    public void ResetComboCountChangesValueCorrectly()
    {
        int expected = 0;
        _playerCharacter.ResetComboCount();
        Assert.AreEqual(expected, _playerCharacter.ComboCount);
    }


    [Test]
    [TestCase(Move.Type.LightAttack)]
    [TestCase(Move.Type.HeavyAttack)]
    [TestCase(Move.Type.Parry)]
    [TestCase(Move.Type.Grab)]
    [TestCase(Move.Type.Special)]
    [TestCase(null)]
    public void CharacterMoveSetGetMoveByTypeReturnsCorrectMove(Move.Type type)
    {
        var moveSet = _playerCharacter.Character.CharacterMoveSet;
        var move = type switch
        {
            Move.Type.LightAttack => moveSet.LightAttack,
            Move.Type.HeavyAttack => moveSet.HeavyAttack,
            Move.Type.Special => moveSet.Special,
            Move.Type.Grab => moveSet.Grab,
            Move.Type.Parry => moveSet.Parry,
            _ => null,
        };
        Assert.AreEqual(move, moveSet.GetMoveByType(type));
    }

    [Test]
    [TestCase(-2)]
    [TestCase(-1)]
    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    public void PlayerMovementControllerUpdatesPositionCorrectly(int position)
    {
        var controller = _playerCharacter.PlayerMovementController;
        controller.KnockIncrement = 1.25f;
        var expectedX = controller.InitialX - (position * controller.KnockIncrement * (float) controller.Direction);
        _playerCharacter.Position = position;
        controller.UpdatePosition();
        Assert.AreEqual(expectedX, _playerCharacter.transform.position.x);
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        _networkManager.GetComponent<NetworkManager>().Shutdown();
        GameObject.Destroy(_networkManager.gameObject);
        GameObject.Destroy(_testObject.gameObject);
    }

    public bool HasCorrectTargetClientId(ClientRpcParams clientRpcParams, ulong targetClientId)
    {
        return clientRpcParams.Send.TargetClientIds.All<ulong>(id => id == targetClientId);
    }
}
