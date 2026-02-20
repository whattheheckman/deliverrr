using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.ParticleSystem;

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

    private float currentSteerT = 0f; // Track steering ramp up time
    private float currentSteerAngle = 0f; // Actual angle applied

    //control variables
    private float move = 0f;
    private float steerDirection = 0f;
    private bool brakePressed = false;

    private ParticleSystem exhaustParticles;
    private Vector3 lastPos = Vector3.zero;
    private Vector3 currentPos = Vector3.zero;
    private float speed = 0f;

    void Start()
    {
        exhaustParticles = GetComponent<ParticleSystem>();
    }

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
            steerDirection = -1f;
        }
        else if (Keyboard.current.dKey.isPressed)
        {
            steerDirection =  1f;
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

        // Apply steering only when there's movement
        if (currentSpeedInterpolation > 0.05f)
        {
            // Calculate steering build-up rate based on speed (slower at high speeds)
            // As currentSpeedInterpolation goes 0->1, factor goes 1->0.5 (example)
            float speedFactor = Mathf.Lerp(1f, 0.4f, currentSpeedInterpolation);
            
            if (steerDirection != 0)
            {
                // Ramp up steering
                currentSteerT = Mathf.MoveTowards(currentSteerT, 1f, Time.deltaTime * steerSpeed * speedFactor);
            }
            else
            {
                // Return to center (faster than steering into turn usually)
                currentSteerT = Mathf.MoveTowards(currentSteerT, 0f, Time.deltaTime * steerSpeed * 2f);
            }

            // Calculate actual steering angle using curve
            // Note: steerDirection needs to be persistent if no input to unwind correctly, 
            // but for simplicity here we assume we steer towards the last input direction or 0
            float targetSign = steerDirection != 0 ? steerDirection : Mathf.Sign(currentSteerAngle);
            if (currentSteerT == 0) targetSign = 0;

            float curveAmount = steeringCurve.Evaluate(currentSteerT);
            currentSteerAngle = targetSign * curveAmount * maximumSteerAmount;

            // Visual wheel rotation
            // Assuming Z-axis rotation for top-down 2D, or Y-axis for 3D car. 
            // Original code used Z Euler for collision/transform, let's stick to that for wheels.
            leftFrontWheel.transform.localRotation = Quaternion.Euler(0, 0, -1f*currentSteerAngle); 
            rightFrontWheel.transform.localRotation = Quaternion.Euler(0, 0, -1f*currentSteerAngle);
            
            // Apply rotation to vehicle body
            // Rotating the car based on the steering angle and distance moved
            // A simple approximation for turning:
            transform.Rotate(0, 0, -currentSteerAngle * currentSpeedInterpolation * Time.deltaTime * 2f); 
        }
        else
        {
            // Reset steering when stopped
            currentSteerT = 0f;
            currentSteerAngle = 0f;
        }

        transform.Translate(0, moveAmount, 0);

        lastPos = currentPos;
        currentPos = this.transform.position;
        speed = (currentPos - lastPos).magnitude / Time.deltaTime;



        //TODO: particle system amount based on speed
        var emiss = exhaustParticles.emission;
        emiss.rateOverTime = Mathf.Lerp(3, 20, speed / forwardSpeed);
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            forwardSpeed *= .93f;
        } else if (collision.gameObject.CompareTag("Boost")){
            forwardSpeed *= 1.10f;
        }
    }
}
