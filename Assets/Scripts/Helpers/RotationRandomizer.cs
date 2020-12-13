using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationRandomizer : MonoBehaviour
{
    public float minValue;
    public float maxValue;
    public Transform objectToRotate;

    private float[] yValues = { 0, 180 };
    // Start is called before the first frame update
    void Start()
    {
        var newRotation = Quaternion.Euler(objectToRotate.rotation.x, yValues[Random.Range(0, 2)], Random.Range(minValue, maxValue));
        objectToRotate.transform.rotation = newRotation;
    }
}
