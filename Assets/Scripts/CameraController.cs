using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private float yInitialPosition;
    void Awake()
    {
        yInitialPosition = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(
            transform.position.x,
            yInitialPosition,
            transform.position.z
        );
    }
}
