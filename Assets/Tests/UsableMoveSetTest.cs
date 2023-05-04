using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Unity.Netcode;
using System;

public class UsableMoveSetTest
{
    NetworkManager _networkManager;
    GameObject _testObject;
    UsableMoveSet _usableMoveSet;
    byte _initialCheckByte;
    bool _initialized = false;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        if (!_initialized)
        {
            _initialized = true;
            var networkManagerPrefab = Resources.Load("Tests/NetworkManagerTester");
            var testObjectPrefab = (GameObject)Resources.Load("Tests/UsableMoveSetTester");
            var networkManagerObject = (GameObject)MonoBehaviour.Instantiate(networkManagerPrefab);
            _networkManager = networkManagerObject.GetComponent<NetworkManager>();
            _networkManager.StartHost();
            _networkManager.AddNetworkPrefab(testObjectPrefab);
            _testObject = (GameObject)MonoBehaviour.Instantiate(testObjectPrefab);
            _testObject.GetComponent<NetworkObject>().Spawn();
            _usableMoveSet = _testObject.GetComponent<UsableMoveSet>();
            InitializeCheckByte();
            yield return null;
        }
    }

    private void InitializeCheckByte()
    {
        var moveSet = _testObject.GetComponent<PlayerCharacter>().Character.CharacterMoveSet;
        var moveTypeList = Enum.GetValues(typeof(CharacterMove.Type));
        _initialCheckByte = 0;
        foreach (CharacterMove.Type type in moveTypeList)
        {
            byte isUsable = (byte)(moveSet.GetMoveByType(type).UsableByDefault ? 1 : 0);
            byte typeAsByte = (byte)type;
            _initialCheckByte |= (byte)(typeAsByte * isUsable);
        }
    }

    [Test, Order(1)]
    public void InitializesCorrectly()
    {
        Assert.AreEqual(_initialCheckByte, _usableMoveSet.Moves.Value);
    }

    [Test, Order(2)]
    [TestCase(CharacterMove.Type.Special)]
    [TestCase(CharacterMove.Type.Parry)]
    [TestCase(CharacterMove.Type.Grab)]
    [TestCase(CharacterMove.Type.LightAttack)]
    [TestCase(CharacterMove.Type.HeavyAttack)]
    public void CheckEnabledByTypeReturnsCorrectValue(CharacterMove.Type type)
    {
        bool result = _usableMoveSet.CheckEnabledByType(type);
        byte mask = (byte) type;
        bool compare = (_initialCheckByte & mask) == mask;
        Assert.AreEqual(compare, result);
    }

    [Test]
    [TestCase(CharacterMove.Type.Special)]
    [TestCase(CharacterMove.Type.Parry)]
    [TestCase(CharacterMove.Type.Grab)]
    [TestCase(CharacterMove.Type.LightAttack)]
    [TestCase(CharacterMove.Type.HeavyAttack)]
    public void DisablesMoveByTypeCorrectly(CharacterMove.Type type)
    {
        _usableMoveSet.DisableMoveByType(type);
        Assert.IsFalse(_usableMoveSet.CheckEnabledByType(type));
    }

    [Test]
    [TestCase(CharacterMove.Type.Special)]
    [TestCase(CharacterMove.Type.Parry)]
    [TestCase(CharacterMove.Type.Grab)]
    [TestCase(CharacterMove.Type.LightAttack)]
    [TestCase(CharacterMove.Type.HeavyAttack)]
    public void EnablesMoveByTypeCorrectly(CharacterMove.Type type)
    {
        _usableMoveSet.EnableMoveByType(type);
        Assert.IsTrue(_usableMoveSet.CheckEnabledByType(type));
    }


    [OneTimeTearDown]
    public void TearDown()
    {
        _networkManager.GetComponent<NetworkManager>().Shutdown();
        GameObject.Destroy(_networkManager.gameObject);
        GameObject.Destroy(_testObject);
    }
}
