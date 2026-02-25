using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScreenSpaceIndicator : MonoBehaviour
{
    [SerializeField] private float margin = 50f;      // Pixels from the edge
    
    private SpriteRenderer spriteRenderer;
    private Transform target;
    private Camera mainCam;
    private bool indicatorUpdates = true;

    void Awake()
    {
        mainCam = Camera.main;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

    }

    public void SetTarget(Transform newTarget)
    {
        if (target != null)
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
    }

    void Update()
    {
        if (!indicatorUpdates) return;

        // 1. Get screen position
        Vector3 screenPos = mainCam.WorldToScreenPoint(target.position);

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
        Vector3 worldPos = mainCam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, -1f));
        transform.position = worldPos;
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