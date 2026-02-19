using UnityEngine;

public class Speedometer : MonoBehaviour
{
    [SerializeField] private GameObject GameObject_target;
    private Rigidbody2D target;

    [SerializeField] private float maxSpeed = 0.0f; // The maximum speed of the target 

    [SerializeField] private float minSpeedArrowAngle;
    [SerializeField] private float maxSpeedArrowAngle;

    [Header("UI")]
    [SerializeField] private SpriteRenderer speedo; // The speedometer;
    [SerializeField] private GameObject arrow; // The arrow in the speedometer

    private float speed = 0.0f;

    private void Awake()
    {
        target = GameObject_target.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        
        speed = target.linearVelocity.magnitude;

        
        arrow.transform.localEulerAngles.Set(0,0, Mathf.Lerp(minSpeedArrowAngle, maxSpeedArrowAngle, speed / maxSpeed));
        speedo.color = Color.Lerp(Color.white, Color.red, speed / maxSpeed);
    }
}