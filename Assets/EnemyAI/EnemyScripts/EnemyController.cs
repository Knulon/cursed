using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Vector2 _oldDirection;
    private Vector2 _direction;
    private float _velocity = 1f;
    private Vector2 _velocityVector;
    [SerializeField]
    public float maxVelocity = 3f;
    [SerializeField]
    public float acceleration = 4f;
    private bool _isMoving;

    [SerializeField]
    public bool HumanControlled = false;


    // Update is called once per frame
    void Update()
    {
        KeyboardInput();
        transform.Translate(_velocityVector * Time.deltaTime);
        SlowDownUntilStop();
    }

    private void SlowDownUntilStop()
    {
        if (_isMoving)
        {
            _isMoving = false;
            return;
        }
        //_velocity -= acceleration * Time.deltaTime;
        Vector2 normalizedVelocityVector = _velocityVector.normalized;
        normalizedVelocityVector *= (acceleration*2) * Time.deltaTime;
        _velocityVector -= normalizedVelocityVector;
        //_velocity = Mathf.Clamp(_velocity, 0, maxVelocity);
    }

    public void Move()
    {
        _isMoving = true;
        // If direction changed a lot prioritize the new direction
        if (Vector2.Angle(_oldDirection, _direction) >= 90)
        {
            Vector2 normalizedVelocityVector = _velocityVector.normalized;
            normalizedVelocityVector *= (acceleration * 2) * Time.deltaTime;
            _velocityVector -= normalizedVelocityVector;
        }

        // Speed up
        //_velocity += acceleration * Time.deltaTime;
        _velocityVector += _direction * acceleration * Time.deltaTime;

        // Clamp velocity
        //_velocity = Mathf.Clamp(_velocity, 0, maxVelocity);
        _velocityVector = Vector2.ClampMagnitude(_velocityVector, maxVelocity);
    }

    public void KeyboardInput()
    {
        if (!HumanControlled)
        {
            return;
        }

        bool keyDown = false;
        Vector2 newDir = Vector2.zero;

        if (Input.GetKey(KeyCode.W))
        {
            newDir += Vector2.up;
            keyDown = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            newDir += Vector2.down;
            keyDown = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            newDir += Vector2.left;
            keyDown = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            newDir += Vector2.right;
            keyDown = true;
        }

        if (keyDown)
        {
            Direction = newDir;
            Move();
        }
    }

    public Vector2 Direction
    {
        get => _direction;
        set
        {
            _oldDirection = _direction;
            _direction = value;
            _direction.Normalize();
        }
    }

    public float Velocity
    {
        get => _velocity;
    }

    public float MaxVelocity
    {
        get => maxVelocity;
    }

    public float Acceleration
    {
        get => acceleration;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(_direction*maxVelocity));

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)_velocityVector);
    }
}
