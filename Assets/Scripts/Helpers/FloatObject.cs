using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatObject : MonoBehaviour
{
    public float amplitude = 0.25f;
    public float speed = 1f;

    private float startingHeight;

    void Start()
    {
        startingHeight = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        var newPosition = new Vector3(transform.position.x, startingHeight + Mathf.Sin(Time.time * speed) * amplitude, transform.position.z);
        transform.position = newPosition;
    }
}
