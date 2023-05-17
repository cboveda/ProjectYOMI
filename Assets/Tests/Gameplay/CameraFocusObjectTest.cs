using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CameraFocusObjectTest
{
    GameObject _testObject;
    List<GameObject> _targets;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var prefab = (GameObject) Resources.Load("Tests/CameraFocusObjectTester");
        _testObject = GameObject.Instantiate(prefab);

        _targets = new List<GameObject>();

        var target1 = new GameObject();
        target1.transform.position = Vector3.left;
        _targets.Add(target1);

        var target2 = new GameObject();
        target2.transform.position = Vector3.right;
        _targets.Add(target2);

        var target3 = new GameObject();
        target3.transform.position = Vector3.up;
        _targets.Add(target3);
    }

    [SetUp]
    public void SetUp()
    {
        _testObject.transform.position = Vector3.zero;
    }

    [UnityTest]
    public IEnumerator CameraFocusObjectMovesToPositionInCenterOfOneTargets()
    {
        Vector3 expected = _targets[0].transform.position;
        _testObject.GetComponent<CameraFocusObject>().AddTarget(_targets[0].transform);
        yield return null;
        Assert.AreEqual(expected, _testObject.transform.position);
    }

    [UnityTest]
    public IEnumerator CameraFocusObjectMovesToPositionInCenterOfTwoTargets()
    {
        Vector3 expected = Vector3.zero;
        expected += _targets[0].transform.position;
        expected += _targets[1].transform.position;
        expected /= 2;
        _testObject.GetComponent<CameraFocusObject>().AddTarget(_targets[0].transform);
        _testObject.GetComponent<CameraFocusObject>().AddTarget(_targets[1].transform);
        yield return null;
        Assert.AreEqual(expected, _testObject.transform.position);
    }

    [UnityTest]
    public IEnumerator CameraFocusObjectMovesToPositionInCenterOfThreeTargets()
    {
        Vector3 expected = Vector3.zero;
        expected += _targets[0].transform.position;
        expected += _targets[1].transform.position;
        expected += _targets[2].transform.position;
        expected /= 3;
        _testObject.GetComponent<CameraFocusObject>().AddTarget(_targets[0].transform);
        _testObject.GetComponent<CameraFocusObject>().AddTarget(_targets[1].transform);
        _testObject.GetComponent<CameraFocusObject>().AddTarget(_targets[2].transform);
        yield return null;
        Assert.AreEqual(expected, _testObject.transform.position);
    }


    [TearDown]
    public void TearDown()
    {
        foreach (var t in _targets)
        {
            _testObject.GetComponent<CameraFocusObject>().RemoveTarget(t.transform);
        }
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        GameObject.Destroy(_testObject);
        for (int i =  _targets.Count - 1; i >= 0; i--)
        {
            GameObject.Destroy(_targets[i]);
        }
    }
}
