using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Unity.Netcode;
using System;
using System.Linq;

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
        var moveTypeList = Enum.GetValues(typeof(Move.Type));
        _initialCheckByte = 0;
        foreach (Move.Type type in moveTypeList)
        {
            if (type == Move.Type.None)
            {
                continue;
            }
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
    [TestCase(Move.Type.Special)]
    [TestCase(Move.Type.Parry)]
    [TestCase(Move.Type.Grab)]
    [TestCase(Move.Type.LightAttack)]
    [TestCase(Move.Type.HeavyAttack)]
    public void CheckEnabledByTypeReturnsCorrectValue(Move.Type type)
    {
        bool result = _usableMoveSet.CheckEnabledByType(type);
        byte mask = (byte) type;
        bool compare = (_initialCheckByte & mask) == mask;
        Assert.AreEqual(compare, result);
    }

    [Test]
    [TestCase(Move.Type.Special)]
    [TestCase(Move.Type.Parry)]
    [TestCase(Move.Type.Grab)]
    [TestCase(Move.Type.LightAttack)]
    [TestCase(Move.Type.HeavyAttack)]
    public void DisablesMoveByTypeCorrectly(Move.Type type)
    {
        _usableMoveSet.DisableMoveByType(type);
        Assert.IsFalse(_usableMoveSet.CheckEnabledByType(type));
    }

    [Test]
    [TestCase(Move.Type.Special)]
    [TestCase(Move.Type.Parry)]
    [TestCase(Move.Type.Grab)]
    [TestCase(Move.Type.LightAttack)]
    [TestCase(Move.Type.HeavyAttack)]
    public void EnablesMoveByTypeCorrectly(Move.Type type)
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
