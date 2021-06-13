using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public float MovementSpeed = 0.05f;
    public float DirectionAngle = 0;
    public bool isDiving = false;
    public AnimationCurve divingCurve;
    public Vector2 diveEvery;
    public float diveTime;
    private bool _isDiving;
    private float _diveStartTime;
    private float _nextDiveTime;
    private float _lastDive;
    private Vector3 _velocity;
    private Vector3 _startPosition;
    // Start is called before the first frame update
    void Start()
    {
        _velocity = transform.forward * MovementSpeed;
        _startPosition = GetComponent<Rigidbody>().position;
        _nextDiveTime = Random.Range(diveEvery.x, diveEvery.y) + Time.time;
        _lastDive = Time.time;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var rb = GetComponent<Rigidbody>();
        rb.velocity = _velocity;
        if (!isDiving)
            return;
        if (_nextDiveTime > Time.time && !_isDiving)
        {
            _isDiving = true;
        }
        if (_isDiving && (_nextDiveTime + diveTime) < Time.time)
        {
            _nextDiveTime = Random.Range(diveEvery.x, diveEvery.y) + Time.time;
            _isDiving = false;
        }
        if (_isDiving)
        {
            float y = divingCurve.Evaluate((Time.time - _nextDiveTime) / diveTime);
            rb.position = new Vector3(rb.position.x, _startPosition.y - (1 - y), rb.position.z);
        }
    }
}
