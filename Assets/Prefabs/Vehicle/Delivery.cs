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
        if (packageManager == null)
        {
            Debug.LogError("Package manager is null! can't pickup or drop off packages!");

        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Pick up package
        if (collision.CompareTag("Packages") && !hasPackage)
        {
            int packageIndice = collision.GetComponent<Package>().getPackageID();
            Debug.Log("Picked up package");
            hasPackage = true;
            spriteRenderer.color = packageColor;
            if (packageManager != null)
            {
                packageManager.pickupPackage(packageIndice);
                
            } else
            {
                Debug.LogError("Package manager is null! can't pickup package");
            }

        }

        // Deliver to customer
        if (collision.CompareTag("Dropzone") && hasPackage)
        {
            int dropzoneIndice = collision.GetComponent<Package>().getPackageID();
            Debug.Log("Delivered package!");
            hasPackage = false;
            spriteRenderer.color = normalColor;
            if (packageManager != null)
            {
                packageManager.dropoffPackage(dropzoneIndice);
            } else
            {
                Debug.LogError("Package manager is null! can't pickup package");
            }
        }
    }

}