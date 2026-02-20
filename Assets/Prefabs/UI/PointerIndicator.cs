using UnityEngine;
using UnityEngine.UI;

public class ScreenSpaceIndicator : MonoBehaviour
{
    [SerializeField] private float margin = 50f; // Pixels from the edge
    private Transform target;
    private Camera mainCam;
    private RectTransform rectTransform;
    private Image iconImage;

    void Awake()
    {
        mainCam = Camera.main;
        rectTransform = GetComponent<RectTransform>();
        iconImage = GetComponent<Image>();
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        // Hide icon if there is no target
        iconImage.enabled = (target != null);
    }

    void Update()
    {
        if (target == null) return;

        // 1. Get screen position
        Vector3 screenPos = mainCam.WorldToScreenPoint(target.position);

        // 2. Check if target is behind the camera (relevant for 3D, safe for Top-Down)
        bool isOffScreen = screenPos.z < 0 ||
                           screenPos.x <= margin ||
                           screenPos.x >= Screen.width - margin ||
                           screenPos.y <= margin ||
                           screenPos.y >= Screen.height - margin;

        if (isOffScreen)
        {
            // Flip position if behind camera
            if (screenPos.z < 0) screenPos *= -1;

            // 3. Clamp to screen bounds with margin
            screenPos.x = Mathf.Clamp(screenPos.x, margin, Screen.width - margin);
            screenPos.y = Mathf.Clamp(screenPos.y, margin, Screen.height - margin);

            // 4. Rotate arrow to point toward the actual target position
            RotateArrow(screenPos);
        }
        else
        {
            // Optional: Reset rotation when on screen, or keep it pointing
            rectTransform.localRotation = Quaternion.identity;
        }

        // 5. Apply position
        rectTransform.position = screenPos;
    }

    private void RotateArrow(Vector3 indicatorPos)
    {
        Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Vector3 direction = (indicatorPos - center).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rectTransform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        // -90 assumes your arrow graphic points "Up" by default
    }
}