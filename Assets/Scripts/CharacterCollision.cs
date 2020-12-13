using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCollision : MonoBehaviour
{
    private GameController gameController;

    void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        var collisionObject = collision.gameObject;
        if (collisionObject.tag == "Deadly")
        {
            transform.parent.GetComponent<CharacterMovement>().Die();
        }
        
    }
    private void OnTriggerEnter(Collider other)
    {
        var collisionObject = other.gameObject;
        if (collisionObject.tag == "Carrot")
        {
            gameController.OnCarrotPickedUp();
            transform.parent.GetComponent<CharacterMovement>().Respawn();
            Destroy(collisionObject.transform.parent.gameObject);
        }
        if (collisionObject.tag == "Deadly")
        {
            transform.parent.GetComponent<CharacterMovement>().Die();
        }
    }
}
