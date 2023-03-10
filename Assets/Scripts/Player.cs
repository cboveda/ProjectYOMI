using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Player : NetworkBehaviour
{
    const float SPEED = 0.1f;
    
    void HandleMovement()
    {
        if (isLocalPlayer)
        {
            float moveHorizontal = Input.GetAxis("Horizontal") * SPEED;
            float moveVertical = Input.GetAxis("Vertical") * SPEED;
            Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0);
            transform.position = transform.position + movement;
        }
    }

    void Update()
    {
        HandleMovement();
    }
}
