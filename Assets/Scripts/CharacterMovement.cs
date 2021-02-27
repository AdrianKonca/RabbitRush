using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class CharacterMovement : MonoBehaviour
{
    private PlayerInput inputs;
    private PlayerInputs playerInputs;
    public float movementSpeed = 5f;
    private string VERTICAL_AXIS = "Vertical";
    private string HORIZONAL_AXIS = "Horizontal";
    private Vector3 startPosition;
    private float POSITION_MAX_DELTA = 0.01f;
    private float AXIS_MINIMUM_MOVEMENT = 0.15f;
    public bool isMovementAllowed = true;
    private bool isMoving;
    private Vector3 moveStartPosition;
    private Vector3 moveTargetPositon;
    private Vector3 movementDirection;
    private PlatformInformation platform;
    private float PLATFORM_OFFSET = 0.60f;
    private bool isOnPlatform;
    private Transform rabbitModel;
    private float finalRotation;

    public Camera playerCamera;
    private bool isDying;
    private float timeLeftToRespawn;
    public float deathAnimationLength = 3f;
    private float deathStartHeight;
    private bool stickReturnedNeutral;

    private GameController gameController;
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == "RabbitModel")
            {
                rabbitModel = transform.GetChild(i);
                break;
            }
        }
        startPosition = transform.position;
        gameController = FindObjectOfType<GameController>();
        Spawn();

    }

    private void Awake()
    {
        playerInputs = new PlayerInputs();
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        var isKeyboard = context.control.device is Keyboard;
        if (!isKeyboard)
            Debug.Log("Handle sticks");
        if ((!context.performed && isKeyboard) || !isMovementAllowed)
            return;

        var direction = context.ReadValue<Vector2>();
        if (!isKeyboard)
        {
            if (direction.magnitude < 1)
            { 
                if (direction.magnitude < 0.2)
                    stickReturnedNeutral = true;
                return;
            }
            if (!stickReturnedNeutral)
                return;
            if (direction.magnitude >= 1)
            {
                stickReturnedNeutral = false;
            }
        }
        
        var sideMovement = direction.x;
        var horizontalMovement = direction.y;
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
            finalRotation = Mathf.Rad2Deg * Mathf.Atan2(movementDirection.x, movementDirection.z);
            moveTargetPositon = transform.position + movementDirection;
            if (Map.IsPositionInBounds(moveTargetPositon) && Map.IsTileFree(moveTargetPositon))
            {
                AudioManager.Instance.PlaySound(AudioManager.Sounds.Movement);
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
                        if (Vector3.Distance(platform.FuturePosition, moveTargetPositon) > PLATFORM_OFFSET)
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
                        var previousPlatform = platform;
                        platform = getNextPlatform(movementDirection);
                        if (Vector3.Distance(platform.FuturePosition, moveTargetPositon) > PLATFORM_OFFSET || platform.Platform == previousPlatform.Platform)
                        {
                            platform = null;
                            moveTargetPositon = getGridPosition(transform.position) + movementDirection * 1.25f;
                            moveTargetPositon.y = -0.2f;
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

                //set rabbit's rotation;
                var angle = Mathf.Rad2Deg * Mathf.Atan2(movementDirection.x, movementDirection.z);
                rabbitModel.rotation = Quaternion.Euler(0, angle, 0);
            }
        }
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

    float SimplifiedBezierQuadratic(float t, float startHeight, float middleForce, float endHeight)
    {
        var p1 = new Vector2(0.5f, middleForce);
        var p2 = new Vector2(1f, endHeight - startHeight);

        var point = 2 * (1 - t) * t * p1 + Mathf.Pow(t, 2) * p2;

        return point.y + startHeight;
    }

    private PlatformInformation getNextPlatform(Vector3 offset)
    {
        var platforms = GameObject.FindGameObjectsWithTag("Platform");
        //retrieve platform objects from map that we are allowed to jump on
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
        if (platformPositions.Count() == 0)
            return null;

        Debug.DrawLine(transform.position, platformPositions[0].FuturePosition, new Color(255, 0, 0));
        return platformPositions[0];
    }
    public void Die()
    {
        deathStartHeight = transform.position.y;
        isDying = true;
        timeLeftToRespawn = deathAnimationLength;
        rabbitModel.GetComponent<BoxCollider>().enabled = false;
        isMovementAllowed = false;
        AudioManager.Instance.PlaySound(AudioManager.Sounds.Death);
        gameController.OnDeath();
    }
    private void FixedUpdate()
    {
        if (!Map.IsPositionInBounds(transform.position) && !isDying)
        {
            Die();
            return;
        }
        if (isDying)
        {
            timeLeftToRespawn -= Time.deltaTime;
            if (timeLeftToRespawn < 0)
            {
                Respawn();
                return;
            }
            var newPosition = new Vector3(transform.position.x, SimplifiedBezierQuadratic(1 - timeLeftToRespawn / deathAnimationLength, deathStartHeight, 3, -2), transform.position.z);
            transform.position = newPosition;
        }
        else if (isMoving)
        {
            var totalDistanceVector = moveTargetPositon - moveStartPosition;
            totalDistanceVector.y = 0;
            
            var newPosition = transform.position + movementDirection * movementSpeed * Time.fixedDeltaTime;
            var distanceMovedVector = moveStartPosition - newPosition;
            distanceMovedVector.y = 0;
            var progression = distanceMovedVector.magnitude / totalDistanceVector.magnitude;

            //print(progression + ":" + moveStartPosition.y + ":" + moveTargetPositon.y);
            newPosition.y = SimplifiedBezierQuadratic(progression, moveStartPosition.y, 1, moveTargetPositon.y);

            var positionOnTwoAxes = transform.position;
            positionOnTwoAxes.y = 0f;

            var ray = new Ray(transform.position, newPosition - transform.position);
            var distance = Vector3.Cross(ray.direction, moveTargetPositon - ray.origin).magnitude;

            if (distance < POSITION_MAX_DELTA)
            {
                isMoving = false;
                rabbitModel.rotation = Quaternion.Euler(0, finalRotation, 0);
                transform.position = moveTargetPositon;
                isOnPlatform = platform != null;
                if ((!isOnPlatform) && Map.IsTileEmpty(transform.position) && Map.GetRowInformation(transform.position) == Map.TileType.Platform)
                    Die();
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
        transform.position = startPosition;
        moveTargetPositon = transform.position;
        rabbitModel.GetComponent<BoxCollider>().enabled = true;
        isMoving = false;
        isMovementAllowed = true;
        isDying = false;
        isOnPlatform = false;
        platform = null;
    }

    public void Spawn()
    {
        Respawn();
        isMovementAllowed = true; //false
    }

    public void SetMovement(bool value)
    {
        isMovementAllowed = value;
    }
    private void OnTriggerEnter(Collider other)
    {
        print("Test2");
    }
    public void InitializePlayer(PlayerConfiguration config)
    {
        inputs = config.Input;
        config.Input.camera = playerCamera;
        config.Input.onActionTriggered += OnActionTriggered;
    }
    private void OnActionTriggered(CallbackContext context)
    {
        if (context.action.name == playerInputs.Player.Move.name)
            OnMove(context);
    }
}
