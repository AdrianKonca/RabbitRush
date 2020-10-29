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
    private bool isMoving = false;
    private Vector3 nextPosition;
    void Start()
    {
        nextPosition = this.transform.position;

    }
    void Update()
    {
        this.transform.position = Vector3.MoveTowards(this.transform.position, nextPosition, Time.deltaTime * MOVEMENT_SPEED);
        var horizontalMovement = Input.GetAxis(HORIZONAL_AXIS);
        var sideMovement = Input.GetAxis(VERTICAL_AXIS);
        print(sideMovement + " " + horizontalMovement);
        if (isMoving && Vector3.Distance(nextPosition, transform.position) < POSITION_MAX_DELTA)
            isMoving = false;
        if (!isMoving && (Mathf.Abs(sideMovement) + Mathf.Abs(horizontalMovement)) > AXIS_MINIMUM_MOVEMENT)
        {
            if (Mathf.Abs(sideMovement) > Mathf.Abs(horizontalMovement))
            {
                nextPosition += new Vector3(Mathf.Sign(sideMovement), 0, 0);
            }
            else
            {
                nextPosition += new Vector3(0, 0, -Mathf.Sign(horizontalMovement));
            }
            isMoving = true;
        }
    }
}
