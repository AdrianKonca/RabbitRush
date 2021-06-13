using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class CharacterMovement : MonoBehaviour
{
    private MapManager map;
    private PlayerInput inputs;
    private PlayerInputs playerInputs;
    public float movementSpeed = 5f;
    private Vector3 startPosition;
    private float POSITION_MAX_DELTA = 0.01f;
    private float AXIS_MINIMUM_MOVEMENT = 0.15f;
    public bool isMovementAllowed = true;
    private bool isMoving;
    private Move move;
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
        map = MapManager.Instance;
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
        try
        {
            GetComponent<PlayerInput>().onActionTriggered += OnActionTriggered;
        }
        catch (System.Exception)
        {

        }
    }

    private void Awake()
    {
        playerInputs = new PlayerInputs();
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        var isKeyboard = context.control.device is Keyboard;

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
        var nextMove = new Move();
        if (!isMoving && (Mathf.Abs(horizontalMovement) + Mathf.Abs(sideMovement)) > AXIS_MINIMUM_MOVEMENT)
        {
            if (Mathf.Abs(horizontalMovement) > Mathf.Abs(sideMovement))
            {
                nextMove.Direction = new Vector3(Mathf.Sign(horizontalMovement), 0, 0);
            }
            else
            {
                nextMove.Direction = new Vector3(0, 0, -Mathf.Sign(sideMovement));
            }
            finalRotation = Mathf.Rad2Deg * Mathf.Atan2(nextMove.Direction.x, nextMove.Direction.z);
            nextMove.Target = transform.position + nextMove.Direction;
            nextMove.Start = transform.position;
            map.ProcessNextMove(nextMove, move, movementSpeed);
            if (nextMove.IsSuccessful)
            {
                var angle = Mathf.Rad2Deg * Mathf.Atan2(nextMove.Direction.x, nextMove.Direction.z);
                AudioManager.Instance.PlaySound(AudioManager.Sounds.Movement);
                rabbitModel.rotation = Quaternion.Euler(0, angle, 0);
                isMoving = true;
                move = nextMove;
            }
            
        }
    }
    float SimplifiedBezierQuadratic(float t, float startHeight, float middleForce, float endHeight)
    {
        var p1 = new Vector2(0.5f, middleForce);
        var p2 = new Vector2(1f, endHeight - startHeight);

        var point = 2 * (1 - t) * t * p1 + Mathf.Pow(t, 2) * p2;

        return point.y + startHeight;
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
            return;
        }

        if (!map.IsInBounds(transform.position))
        {
            Die();
            return;
        }
        else if (isMoving)
        {
            var totalDistanceVector = move.Target - move.Start;
            totalDistanceVector.y = 0;
            
            var newPosition = transform.position + move.Direction * movementSpeed * Time.fixedDeltaTime;
            var distanceMovedVector = move.Start - newPosition;
            distanceMovedVector.y = 0;
            var progression = distanceMovedVector.magnitude / totalDistanceVector.magnitude;

            newPosition.y = SimplifiedBezierQuadratic(progression, move.Start.y, 1, move.Target.y);

            var positionOnTwoAxes = transform.position;
            positionOnTwoAxes.y = 0f;

            var ray = new Ray(transform.position, newPosition - transform.position);
            var distance = Vector3.Cross(ray.direction, move.Target - ray.origin).magnitude;

            if (distance < POSITION_MAX_DELTA)
            {
                isMoving = false;
                rabbitModel.rotation = Quaternion.Euler(0, finalRotation, 0);
                transform.position = move.Target;
                if ((move.Platform != null) && !map.IsInBounds(transform.position))
                    Die();
            }
            else
            {
                transform.position = newPosition;
            }
        }
        else
        {
            if (move?.Platform != null)
                transform.position = move.Platform.Platform.position;
        }
        if (transform.position.y <= -0.1 && !isDying)
        {
            Die();
            return;
        }

    }
    public void Respawn()
    {
        transform.position = startPosition;
        rabbitModel.GetComponent<BoxCollider>().enabled = true;
        isMoving = false;
        isMovementAllowed = true;
        isDying = false;
        move = null;
    }

    public void Spawn()
    {
        Respawn();
        isMovementAllowed = false;
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
        SetPlayerInput(config.Input);
    }

    public void SetPlayerInput(PlayerInput input)
    {
        inputs = input;
        inputs.camera = playerCamera;
        inputs.onActionTriggered += OnActionTriggered;
        inputs.onActionTriggered += GameController.Instance.OnActionTriggered;
    }
    private void OnActionTriggered(CallbackContext context)
    {
        if (context.action.name == playerInputs.Player.Move.name)
            OnMove(context);
    }
}
