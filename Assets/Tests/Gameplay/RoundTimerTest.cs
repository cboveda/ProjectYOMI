using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class RoundTimerTest
{
    RoundTimer _roundTimer;
    
    [OneTimeSetUp]
    public void SetUp()
    {
        var prefab = Resources.Load("Tests/RoundTimerTester");
        var roundTimerObject = (GameObject) MonoBehaviour.Instantiate(prefab);
        _roundTimer = roundTimerObject.GetComponent<RoundTimer>();
    }

    [UnityTest]
    public IEnumerator StartTimerActivatesTimer()
    {
        _roundTimer.StartTimer(1f);
        var intial = _roundTimer.CurrentTimeSeconds;
        yield return new WaitForSeconds(0.1f);
        var current = _roundTimer.CurrentTimeSeconds;
        Assert.IsTrue(current < intial);
    }

    [UnityTest]
    public IEnumerator TimerActiveIsFalseAfterTimerCompletes()
    {
        _roundTimer.StartTimer(0.01f);
        yield return new WaitForSeconds(0.02f);
        Assert.IsFalse(_roundTimer.TimerActive);
    }

    [OneTimeTearDown] 
    public void TearDown()
    {
        GameObject.Destroy(_roundTimer.gameObject);
    }
}
