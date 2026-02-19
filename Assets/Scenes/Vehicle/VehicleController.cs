using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class VehicleController : MonoBehaviour
{
    [Header("Acceleration")]
    [SerializeField] private float forwardSpeed = 10f;
    [SerializeField] private AnimationCurve accelerationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float accelerationDuration = 3f;


    [Header("Steering")]
    [SerializeField] private GameObject leftFrontWheel;
    [SerializeField] private GameObject rightFrontWheel;
    [SerializeField] private AnimationCurve steeringCurve = AnimationCurve.EaseInOut(0,0, 1, 1);
    [SerializeField] private float maximumSteerAmount = 35f; 
    [SerializeField] private float steerSpeed = 2f;




    [Header("Braking")]
    [SerializeField] private AnimationCurve brakingCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    [SerializeField] private float brakingDuration = 0.5f;



    private float currentSpeedInterpolation = 0f;
    private bool isBraking = false;

    private float curveValue;
    private float brakingProgress = 0f;
    private float interpolatedSpeed = 0f;
    private float moveAmount = 0f;

    private float lastSteerAmount = 0f;
    private float steerAmount = 0f;


    //control variables
    private float move = 0f;
    private float steerDirection = 0f;
    private bool brakePressed = false;

    // Update is called once per frame
    void Update()
    {
        // Reset current inputs at beginning of every frame
        move = 0f;
        steerDirection = 0f;
        brakePressed = false;

        //decide to move forward or not
        if (Keyboard.current.wKey.isPressed)
        {
            move = 1f;
        }

        // check for steering
        if (Keyboard.current.aKey.isPressed)
        {
            steerDirection = 1f;
        }
        else if (Keyboard.current.dKey.isPressed)
        {
            steerDirection = -1f;
        }

        // Check for brake input (s key)
        if (Keyboard.current.sKey.isPressed)
        {
            brakePressed = true;
        }

        // Interpolate speed using the acceleration or braking curve
        if (brakePressed && currentSpeedInterpolation > 0f)
        {
            // Apply braking - faster deceleration
            isBraking = true;
            currentSpeedInterpolation = Mathf.MoveTowards(currentSpeedInterpolation, 0f, Time.deltaTime / brakingDuration);
        }
        else if (move > 0)
        {
            // Accelerate
            isBraking = false;
            currentSpeedInterpolation = Mathf.MoveTowards(currentSpeedInterpolation, move, Time.deltaTime / accelerationDuration);
        }
        else
        {
            // Natural deceleration (no input)
            isBraking = false;
            currentSpeedInterpolation = Mathf.MoveTowards(currentSpeedInterpolation, 0f, Time.deltaTime / accelerationDuration);
        }

        // use accel curves
        if (isBraking)
        {
            // Braking curve: starts at 1 (full speed) and curves down to 0
            brakingProgress = 1f - (currentSpeedInterpolation / Mathf.Max(0.01f, currentSpeedInterpolation + Time.deltaTime / brakingDuration));
            curveValue = brakingCurve.Evaluate(brakingProgress) * currentSpeedInterpolation;
        }
        else
        {
            // Normal acceleration curve
            curveValue = accelerationCurve.Evaluate(currentSpeedInterpolation);
        }

        // After choosing whether to brake or accel, then calc the actual move amount
        interpolatedSpeed = curveValue * forwardSpeed;
        moveAmount = interpolatedSpeed * Time.deltaTime;



        steerAmount = 0f;
        // Apply steering only when there's movement, scaled by current speed interpolation
        if (currentSpeedInterpolation > 0.05f)
        {
            // steer is left or right direction
            steerAmount = steerDirection * Mathf.MoveTowards(Mathf.Abs(lastSteerAmount), 1, Time.deltaTime / steerSpeed);

            // rotation should be scaled to maximum steer amount where maximum stter amount 

            leftFrontWheel.transform.localRotation = Quaternion.Euler(0, 0, steeringCurve.Evaluate(steerAmount / maximumSteerAmount)); // Rotate left wheel
            rightFrontWheel.transform.localRotation = Quaternion.Euler(0, 0, steeringCurve.Evaluate(steerAmount / maximumSteerAmount)); // Rotate right wheel
        }
        lastSteerAmount = steerAmount;
        transform.Rotate(0, 0, steerAmount);
        transform.Translate(0, moveAmount, 0);
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Bumped into something!");
        //currentSpeed = slowSpeed;
    }
}
