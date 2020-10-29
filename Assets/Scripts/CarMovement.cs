using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public float MovementSpeed = 0.05f;
    public float DirectionAngle;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position += this.transform.forward * MovementSpeed * Time.deltaTime;
        //var vForce = Quaternion.AngleAxis(DirectionAngle, Vector3.forward) * Vector3.right + new Vector3(0f, 0.5f, 0f);
        //Debug.DrawLine(this.transform.position, vForce);
    }
}
