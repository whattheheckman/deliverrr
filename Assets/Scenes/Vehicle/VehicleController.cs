using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
//using UnityEngine.Sprite;

public class VehicleController : MonoBehaviour
{
    [Header("VehicleSettings")]
    [SerializeField] private float forwardSpeed = 10f;
    [SerializeField] private AnimationCurve accelerationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float accelerationDuration = 3f;


    [Header("Steering")]
    [SerializeField] private float steerSpeed = 0.5f;
    [SerializeField] private GameObject leftFrontWheel;
    [SerializeField] private GameObject rightFrontWheel;


    [Header("Braking")]
    [SerializeField] private AnimationCurve brakingCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    [SerializeField] private float brakingDuration = 0.5f;



    private float currentSpeedInterpolation = 0f;
    private float targetSpeedDirection = 0f;
    private bool isBraking = false;

    // Update is called once per frame
    void Update()
    {
        float move = 0f;
        float steer = 0f;
        bool brakePressed = false;

        // Get movement input (forward only)
        if (Keyboard.current.wKey.isPressed)
        {
            move = 1f;
        }
        // Get steering input (independent of movement keys)
        if (Keyboard.current.aKey.isPressed)
        {
            steer = 1f;
        }
        else if (Keyboard.current.dKey.isPressed)
        {
            steer = -1f;
        }

        // Check for brake input (s key)
        if (Keyboard.current.sKey.isPressed)
        {
            brakePressed = true;
        }

        // Interpolate speed using the acceleration or braking curve
        targetSpeedDirection = move;
        float targetInterpolation = Mathf.Abs(targetSpeedDirection);

        if (brakePressed && currentSpeedInterpolation > 0f)
        {
            // Apply braking - faster deceleration
            isBraking = true;
            currentSpeedInterpolation = Mathf.MoveTowards(currentSpeedInterpolation, 0f, Time.deltaTime / brakingDuration);
        }
        else if (targetInterpolation > 0)
        {
            // Accelerate
            isBraking = false;
            currentSpeedInterpolation = Mathf.MoveTowards(currentSpeedInterpolation, targetInterpolation, Time.deltaTime / accelerationDuration);
        }
        else
        {
            // Natural deceleration (no input)
            isBraking = false;
            currentSpeedInterpolation = Mathf.MoveTowards(currentSpeedInterpolation, 0f, Time.deltaTime / accelerationDuration);
        }

        // Apply the appropriate curve
        float curveValue;
        if (isBraking)
        {
            // Braking curve: starts at 1 (full speed) and curves down to 0
            float brakingProgress = 1f - (currentSpeedInterpolation / Mathf.Max(0.01f, currentSpeedInterpolation + Time.deltaTime / brakingDuration));
            curveValue = brakingCurve.Evaluate(brakingProgress) * currentSpeedInterpolation;
        }
        else
        {
            // Normal acceleration curve
            curveValue = accelerationCurve.Evaluate(currentSpeedInterpolation);
        }

        float interpolatedSpeed = curveValue * forwardSpeed;

        float moveAmount = interpolatedSpeed * Time.deltaTime;

        // Apply steering only when there's movement, scaled by current speed interpolation
        float steerAmount = 0f;

        steerAmount = steer * steerSpeed * (4 / currentSpeedInterpolation) * Time.deltaTime;
        leftFrontWheel.transform.localRotation = Quaternion.Euler(0, 0, steerAmount * 30f); // Rotate left wheel
        rightFrontWheel.transform.localRotation = Quaternion.Euler(0, 0, steerAmount * 30f); // Rotate right wheel

        transform.Translate(0, moveAmount, 0);
        if (currentSpeedInterpolation > 0.01f)
        {
            transform.Rotate(0, 0, steerAmount);
        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Bumped into something!");
        //currentSpeed = slowSpeed;
    }
}
