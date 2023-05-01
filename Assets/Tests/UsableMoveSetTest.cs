using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Unity.Netcode;
using System;

public class UsableMoveSetTest
{
    GameObject _networkManager;
    GameObject _testObject;
    UsableMoveSet _usableMoveSet;
    byte _initialCheckByte;

    [OneTimeSetUp]
    public void SetUp()
    {
        _networkManager = (GameObject) MonoBehaviour.Instantiate(Resources.Load("TestPrefabs/NetworkManagerTester"));
        _testObject = (GameObject) MonoBehaviour.Instantiate(Resources.Load("TestPrefabs/UsableMoveSetTester"));
        _usableMoveSet = _testObject.GetComponent<UsableMoveSet>();

        var moveSet = _testObject.GetComponent<PlayerCharacter>().Character.CharacterMoveSet;
        var moveTypeList = Enum.GetValues(typeof(CharacterMove.Type));
        _initialCheckByte = 0;
        foreach (CharacterMove.Type type in moveTypeList)
        {
            byte isUsable = (byte)(moveSet.GetMoveByType(type).UsableByDefault ? 1 : 0);
            byte typeAsByte = (byte)type;
            _initialCheckByte |= (byte)(typeAsByte * isUsable);
        }

        var manager = _networkManager.GetComponent<NetworkManager>();
        manager.StartHost();
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
        GameObject.Destroy(_networkManager);
        GameObject.Destroy(_testObject);
    }
}
