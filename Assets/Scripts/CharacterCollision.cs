using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        var collisionObject = collision.gameObject;
        if (collisionObject.tag == "Deadly")
        {
            transform.parent.GetComponent<CharacterMovement>().Respawn();
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        var collisionObject = other.gameObject;
        if (collisionObject.tag == "Carrot")
        {
            Application.Quit();
            print("You won!");
            transform.parent.GetComponent<CharacterMovement>().Respawn();
        }
    }
}
