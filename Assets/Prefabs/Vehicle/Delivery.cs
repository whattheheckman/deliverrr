using UnityEngine;

public class Delivery : MonoBehaviour
{
    [Header("Indicator Settings")]
    [SerializeField] Color packageColor = Color.yellow;
    [SerializeField] Color normalColor = Color.white;

    [SerializeField] PackageManager packageManager;
    
    SpriteRenderer spriteRenderer;
    private bool hasPackage = false;


    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Pick up package
        if (collision.CompareTag("Packages") && !hasPackage)
        {
            Debug.Log("Picked up package");
            hasPackage = true;
            spriteRenderer.color = packageColor;
            collision.gameObject.SetActive(false);
        }

        // Deliver to customer
        if (collision.CompareTag("Dropzone") && hasPackage)
        {
            Debug.Log("Delivered package!");
            hasPackage = false;
            spriteRenderer.color = normalColor;
            Destroy(collision.gameObject, Time.deltaTime * 2f);
            if (packageManager != null)
            {
                packageManager.UpdateDeliveredPackageCount();
            }
        }
    }

}