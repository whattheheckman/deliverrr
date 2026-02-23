using UnityEngine;
using UnityEngine.InputSystem;

public class LookAt : MonoBehaviour
{
    private float rotationSpeed = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Handle rotation to follow mouse cursor
        if (Camera.main != null)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePosition.z = 0f;

            Vector2 direction = (mousePosition - transform.position).normalized;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            float currentAngle = this.transform.rotation.eulerAngles.z;

            // Smoothly rotate towards target angle
            float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
           // rb.MoveRotation(newAngle);
            //Debug.DrawLine(rb.position, mousePosition);
        }
    }
}
