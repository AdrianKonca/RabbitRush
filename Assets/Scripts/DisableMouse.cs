using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableMouse : MonoBehaviour
{
    //disable mouse cursor because Unity can't handle both keyboard and mouse menus for local multiplayer...
    private void Awake()
    {
        if (!Debug.isDebugBuild)
            Cursor.lockState = CursorLockMode.Locked;
    }
}
    

