using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        var collisionObject = collision.collider.gameObject;
        if (collisionObject.tag == "Deadly")
        {
            transform.parent.GetComponent<CharacterMovement>().Respawn();
        }
    }
}
