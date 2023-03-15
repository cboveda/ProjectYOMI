using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountdownAnimationLifecycle : MonoBehaviour
{
    public void Close()
    {
        UIManager.Instance.Countdown.gameObject.SetActive(false);
    }
}
