using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RoundSystem : NetworkBehaviour
{
    [SerializeField] private Animator animator = null;

    public void CountdownEnded()
    {
        animator.enabled = false;
    }

}
