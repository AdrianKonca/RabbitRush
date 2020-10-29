using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private string VERTICAL_AXIS = "Vertical";
    private string HORIZONAL_AXIS = "Horizontal";
    private float MOVEMENT_SPEED = 5f;
    private float POSITION_MAX_DELTA = 0.001f;
    private float AXIS_MINIMUM_MOVEMENT = 0.05f;
    private bool isMoving;
    private Vector3 nextPosition;
    void Start()
    {
        Respawn();
    }

    void Update()
    {
        this.transform.position = Vector3.MoveTowards(this.transform.position, nextPosition, Time.deltaTime * MOVEMENT_SPEED);
        var sideMovement = Input.GetAxis(HORIZONAL_AXIS);
        var horizontalMovement = Input.GetAxis(VERTICAL_AXIS);
        if (isMoving && Vector3.Distance(nextPosition, transform.position) < POSITION_MAX_DELTA)
            isMoving = false;
        if (!isMoving && (Mathf.Abs(horizontalMovement) + Mathf.Abs(sideMovement)) > AXIS_MINIMUM_MOVEMENT)
        {
            if (Mathf.Abs(horizontalMovement) > Mathf.Abs(sideMovement))
            {
                nextPosition += new Vector3(Mathf.Sign(horizontalMovement), 0, 0);
            }
            else
            {
                nextPosition += new Vector3(0, 0, -Mathf.Sign(sideMovement));
            }
            isMoving = true;
        }
    }
    private void Respawn()
    {
        transform.position = new Vector3(0, 0.5f, 0);
        nextPosition = transform.position;
        isMoving = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        var collisionObject = collision.collider.gameObject;
        if (collisionObject.name.Contains("deadly"))
            Respawn();
        
    }
}
