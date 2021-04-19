using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterDebug : MonoBehaviour
{
    public PlayerInput PlayerInput;
    public CharacterMovement CharacterMovement;
    void Start()
    {
        CharacterMovement.SetPlayerInput(PlayerInput);
    }
}
