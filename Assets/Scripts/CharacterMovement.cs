using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float movementSpeed = 5f;
    private string VERTICAL_AXIS = "Vertical";
    private string HORIZONAL_AXIS = "Horizontal";
    private float POSITION_MAX_DELTA = 0.01f;
    private float AXIS_MINIMUM_MOVEMENT = 0.15f;
    private bool isMoving;
    private Vector3 moveStartPosition;
    private Vector3 moveTargetPositon;
    private Vector3 movementDirection;
    private PlatformInformation platform;
    private bool isOnPlatform;

    void Start()
    {
        Respawn();
    }

    Vector3 getGridPosition(Vector3 position)
    {

        var gridPosition = new Vector3(Mathf.Round(position.x), position.y, Mathf.Round(position.z));

        Debug.DrawRay(gridPosition, Vector3.left, new Color(255, 0, 0));
        Debug.DrawRay(gridPosition, Vector3.right, new Color(0, 255, 0));
        Debug.DrawRay(gridPosition, Vector3.forward, new Color(0, 0, 255));
        Debug.DrawRay(gridPosition, Vector3.back, new Color(0, 255, 255));
        return gridPosition;
    }

    float SimplifiedBezierQuadratic(float t, float startHeight, float endHeight)
    {
        var p1 = new Vector2(0.5f, 1f);
        var p2 = new Vector2(1f, endHeight - startHeight);

        var point = 2 * (1 - t) * t * p1 + Mathf.Pow(t, 2) * p2;

        return point.y + startHeight;
    }
    void Update()
    {
        var sideMovement = Input.GetAxis(HORIZONAL_AXIS);
        var horizontalMovement = Input.GetAxis(VERTICAL_AXIS);
        //pick direction of move
        if (!isMoving && (Mathf.Abs(horizontalMovement) + Mathf.Abs(sideMovement)) > AXIS_MINIMUM_MOVEMENT)
        {
            if (Mathf.Abs(horizontalMovement) > Mathf.Abs(sideMovement))
            {
                movementDirection = new Vector3(Mathf.Sign(horizontalMovement), 0, 0);
            }
            else
            {
                movementDirection = new Vector3(0, 0, -Mathf.Sign(sideMovement));
            }
            moveTargetPositon = transform.position + movementDirection;
            if (Map.IsVectorInBounds(moveTargetPositon))
            {
                moveStartPosition = transform.position;
                isMoving = true;
                //if move is going to happen on platform, calculate next position based on platform 

                var currentTileType = Map.GetRowInformation(moveStartPosition);
                var nextTileType = Map.GetRowInformation(moveTargetPositon);

                if (currentTileType == Map.TileType.Grid)
                {
                    if (nextTileType == Map.TileType.Grid)
                    {
                        platform = null;
                        moveTargetPositon = getGridPosition(transform.position) + movementDirection;
                        movementDirection = moveTargetPositon - moveStartPosition;
                        movementDirection.y = 0f;
                    }
                    else if (nextTileType == Map.TileType.Platform)
                    {
                        platform = getNextPlatform(movementDirection);
                        if (Vector3.Distance(platform.FuturePosition, moveTargetPositon) > 0.66f)
                        {
                            platform = null;
                            moveTargetPositon = getGridPosition(transform.position) + movementDirection;
                            moveTargetPositon.y -= 0.5f;
                            movementDirection = moveTargetPositon - moveStartPosition;
                        }
                        else
                        {
                            moveTargetPositon = platform.FuturePosition;
                            movementDirection = moveTargetPositon - moveStartPosition;
                            movementDirection.y = 0f;
                        }
                        
                    }
                }
                else if (currentTileType == Map.TileType.Platform)
                {
                    if (nextTileType == Map.TileType.Grid)
                    {
                        platform = null;
                        moveTargetPositon = getGridPosition(moveTargetPositon);
                        moveTargetPositon.y = Map.GetTileHeight();
                        movementDirection = moveTargetPositon - moveStartPosition;
                        movementDirection.y = 0f;
                    }
                    else if (nextTileType == Map.TileType.Platform)
                    {
                        isMoving = false;
                    }
                }
            }
        }
        Debug.DrawLine(transform.position, moveTargetPositon, new Color(200, 200, 0));
        if (isMoving)
            Debug.DrawRay(transform.position, Vector3.up, new Color(0, 255, 0));
        else
            Debug.DrawRay(transform.position, Vector3.up, new Color(255, 0, 0));
        getNextPlatform(Vector3.zero);
    }

    private PlatformInformation getNextPlatform(Vector3 offset)
    {
        var platforms = GameObject.FindGameObjectsWithTag("Platform");
        var platformPositions = new List<PlatformInformation>();
        foreach (var platform in platforms)
        {
            var script = platform.transform.GetComponent<PlatformPositioner>();
            var r = script.GetNextObjectPositions(1f / movementSpeed);
            platformPositions.AddRange(r);            
        }
        foreach (var position in platformPositions)
        {
            Debug.DrawLine(transform.position, position.FuturePosition);
        }
        platformPositions = platformPositions.OrderBy(pos => Vector3.Distance(transform.position + offset, pos.FuturePosition)).ToList();
        Debug.DrawLine(transform.position, platformPositions[0].FuturePosition, new Color(255, 0, 0));
        return platformPositions[0];
    }
    private void FixedUpdate()
    {
        //My version
        if (isMoving)
        {
            var totalDistanceVector = moveTargetPositon - moveStartPosition;
            totalDistanceVector.y = 0;
            
            var newPosition = transform.position + movementDirection * movementSpeed * Time.fixedDeltaTime;
            var distanceMovedVector = moveStartPosition - newPosition;
            distanceMovedVector.y = 0;
            var progression = distanceMovedVector.magnitude / totalDistanceVector.magnitude;

            //print(progression + ":" + moveStartPosition.y + ":" + moveTargetPositon.y);
            newPosition.y = SimplifiedBezierQuadratic(progression, moveStartPosition.y, moveTargetPositon.y);

            var positionOnTwoAxes = transform.position;
            positionOnTwoAxes.y = 0f;

            var ray = new Ray(transform.position, newPosition - transform.position);
            var distance = Vector3.Cross(ray.direction, moveTargetPositon - ray.origin).magnitude;

            if (distance < POSITION_MAX_DELTA)
            {
                isMoving = false;
                transform.position = moveTargetPositon;
                isOnPlatform = platform != null;
                if ((!isOnPlatform) && Map.IsTileEmpty(transform.position) && Map.GetRowInformation(transform.position) == Map.TileType.Platform)
                {
                    Respawn();
                }
            }
            else
            {
                transform.position = newPosition;
            }
        }
        else
        {
            if (isOnPlatform)
                transform.position = platform.Platform.position;
        }

    }
    public void Respawn()
    {
        transform.position = new Vector3(0, 0, 0);
        moveTargetPositon = transform.position;
        isMoving = false;
        isOnPlatform = false;
        platform = null;
    }
}
