using UnityEngine;

public class Speedometer : MonoBehaviour
{
    [SerializeField] private GameObject GameObject_target;

    [SerializeField] private float maxSpeed = 0.0f; // The maximum speed of the target 

    [SerializeField] private float minSpeedArrowAngle;
    [SerializeField] private float maxSpeedArrowAngle;

    [Header("UI")]
    [SerializeField] private SpriteRenderer speedo; // The speedometer;
    [SerializeField] private GameObject arrow; // The arrow in the speedometer
    
    private float speed = 0.0f;

    private float target_lastPos = 0.0f;
    private float target_currentPos = 0.0f;

   
    private void Update()
    {
        target_lastPos = target_currentPos;
        target_currentPos = GameObject_target.transform.position.magnitude;
        speed = (target_currentPos - target_lastPos ) / Time.deltaTime;


        
        
        arrow.transform.localEulerAngles.Set(0,0, Mathf.Lerp(minSpeedArrowAngle, maxSpeedArrowAngle, speed / maxSpeed));
        speedo.color = Color.Lerp(Color.white, Color.red, speed / maxSpeed);
    }
}