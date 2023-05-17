using System.Collections.Generic;
using UnityEngine;

public class CameraFocusObject : MonoBehaviour, ICameraFocusObject
{
    private readonly List<Transform> _targets = new();

    void Update()
    {
        if (_targets.Count == 0)
        {
            return;
        }

        Vector3 cumulativePosition = Vector3.zero;
        foreach (Transform t in _targets)
        {
            cumulativePosition += t.position;
        }
        var targetPosition = cumulativePosition / _targets.Count;
        transform.position = targetPosition;
    }

    public void AddTarget(Transform t)
    {
        _targets.Add(t);
        Debug.Log($"Camera targets ({_targets.Count}): {string.Join(", ", _targets)}");
    }

    public void RemoveTarget(Transform t)
    {
        _targets.Remove(t);
    }
}
