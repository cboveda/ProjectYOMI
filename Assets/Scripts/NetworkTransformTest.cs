using System;
using Unity.Netcode;
using UnityEngine;

public class NetworkTransformTest : NetworkBehaviour
{
    void Update()
    {
        if (IsServer)
        {
            float theta = Time.frameCount / 100.0f;
            transform.position = new Vector3((float)Math.Cos(theta), (float)Math.Sin(theta), 0.0f);
        }
    }
}
