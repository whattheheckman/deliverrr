using UnityEngine;

public class PointerIndicatorController : MonoBehaviour
{
    [SerializeField] private Sprite indicatorSprite;
    [SerializeField] private Color spriteColor;

    private GameObject indicatorObject;
    private ScreenSpaceIndicator indicator;

    void Start()
    {
        CreateIndicator();
    }

    private void CreateIndicator()
    {
        // Create a new GameObject for the indicator
        indicatorObject = new GameObject($"{gameObject.name}_Indicator");
        
        // Add SpriteRenderer component
        SpriteRenderer spriteRenderer = indicatorObject.AddComponent<SpriteRenderer>();
        
        // Set the sprite if provided
        if (indicatorSprite != null)
        {
            spriteRenderer.sprite = indicatorSprite;
        }
        
        // Set material if provided, otherwise use default sprite material
        if (spriteColor != null)
        {
            spriteRenderer.color = spriteColor;
        }

        // Set sorting layer to ensure visibility
        spriteRenderer.sortingLayerID = SortingLayer.NameToID("UI");

        // Add and configure the ScreenSpaceIndicator component
        indicator = indicatorObject.AddComponent<ScreenSpaceIndicator>();
        
        // Use reflection to set private fields if needed, or modify ScreenSpaceIndicator
        // to have public setters. For now, the indicator will use its serialized values.
        
        // Set this GameObject as the target
        indicator.SetTarget(transform);
    }

    void OnDestroy()
    {
        // Clean up the indicator when this object is destroyed
        if (indicatorObject != null)
        {
            Destroy(indicatorObject);
        }
    }
}