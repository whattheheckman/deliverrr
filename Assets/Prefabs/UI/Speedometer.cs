using UnityEngine;
using TMPro;

public class Speedometer : MonoBehaviour
{
    [SerializeField] private GameObject GameObject_target;

    [SerializeField] private float maxSpeed = 0.0f; // The maximum speed of the target 

    [SerializeField] private float minSpeedArrowAngle;
    [SerializeField] private float maxSpeedArrowAngle;

    [Header("UI")]
    [SerializeField] private SpriteRenderer speedoGraphics; // The speedometer;
    [SerializeField] private GameObject arrow; // The arrow in the speedometer\
    [SerializeField] private TextMeshProUGUI speedText;
    
    private float speed = 0.0f;

    private Vector3 target_lastPos = Vector3.zero;
    private Vector3 target_currentPos = Vector3.zero;

   
    private void FixedUpdate()
    {
        target_lastPos = target_currentPos;
        target_currentPos = GameObject_target.transform.position;
        speed = (target_currentPos - target_lastPos).magnitude / Time.deltaTime;


        speedText.text = string.Format("{00:F1}mph", speed);
        arrow.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(minSpeedArrowAngle, maxSpeedArrowAngle, speed / maxSpeed));
        speedoGraphics.color = Color.Lerp(Color.white, Color.red, speed / maxSpeed);
    }
}