using UnityEngine;

public interface ICameraFocusObject
{
    void AddTarget(Transform t);
    void RemoveTarget(Transform t);
}