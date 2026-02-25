using UnityEngine;
using Unity.Cinemachine;

public class ScreenSpaceIndicator : MonoBehaviour
{
    [SerializeField] private float margin = 50f;      // Pixels from the edge
    
    [Header("Scale Settings")]
    [SerializeField] private float minDistance = 60f; // Distance where scale is at minScale
    [SerializeField] private float maxDistance = 0f; // Distance where scale is at maxScale
    [SerializeField] private float minScale = 5f;
    [SerializeField] private float maxScale = 15f;

    private SpriteRenderer spriteRenderer;
    private Transform target;
    private Camera mainCamera;
    private bool indicatorUpdates = true;

    void Awake()
    {
        mainCamera = CinemachineBrain.GetActiveBrain(0).OutputCamera;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void SetTarget(Transform newTarget)
    {
        if (newTarget != null)
        {
            target = newTarget;
            indicatorUpdates = true;
        } else
        {
            indicatorUpdates = false;
        }
        // Hide renderer if there is no target
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = indicatorUpdates;
        }
    }

    public void isEnabled(bool amIenabled)
    {
        indicatorUpdates = amIenabled;
        spriteRenderer.enabled = amIenabled;
    }

    void FixedUpdate()
    {
        if (!indicatorUpdates || target == null) return;

        // 1. Get screen position of the target
        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position);

        // 2. Check if target is off-screen or behind camera
        bool isOffScreen = screenPos.z < 0 ||
                           screenPos.x <= margin ||
                           screenPos.x >= Screen.width - margin ||
                           screenPos.y <= margin ||
                           screenPos.y >= Screen.height - margin;

        if (isOffScreen)
        {
            if (screenPos.z < 0) screenPos *= -1;

            // 3. Clamp to screen bounds
            screenPos.x = Mathf.Clamp(screenPos.x, margin, Screen.width - margin);
            screenPos.y = Mathf.Clamp(screenPos.y, margin, Screen.height - margin);

            RotateArrow(screenPos);
        }
        else
        {
            transform.localRotation = Quaternion.identity;
        }

        // 4. Convert screen position back to World Position for the Sprite
        // We set Z to 10 (or any distance) so it's visible in front of the camera
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 10f));
        transform.position = worldPos;

        // 5. Adjust scale based on distance
        if (spriteRenderer != null)
        {
            // Using Vector2.Distance to ignore Z-axis depth differences in 2D
            float distance = Vector2.Distance(mainCamera.transform.position, target.position);
            
            // Get a 0 to 1 value based on where the distance falls between minDistance and maxDistance
            float normalizedDistance = Mathf.InverseLerp(minDistance, maxDistance, distance);
            
            // Calculate the final scale
            float currentScale = Mathf.Lerp(minScale, maxScale, normalizedDistance);
            
            // Apply the scale
            spriteRenderer.transform.localScale = new Vector3(currentScale, currentScale, 1f);
        }
    }

    private void RotateArrow(Vector3 indicatorPos)
    {
        Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Vector3 direction = (indicatorPos - center).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // -90 assumes your arrow graphic points "Up" by default
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }
}