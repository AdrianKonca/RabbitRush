using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public float MovementSpeed = 0.05f;
    public float DirectionAngle = 0;
    private Vector3 _velocity;
    // Start is called before the first frame update
    void Start()
    {
        _velocity = transform.forward * MovementSpeed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var rb = GetComponent<Rigidbody>();
        rb.velocity = _velocity;
        //this.transform.position += this.transform.forward * MovementSpeed * Time.deltaTime;
        //var vForce = Quaternion.AngleAxis(DirectionAngle, Vector3.forward) * Vector3.right + new Vector3(0f, 0.5f, 0f);
        //Debug.DrawLine(this.transform.position, vForce);
    }
}
