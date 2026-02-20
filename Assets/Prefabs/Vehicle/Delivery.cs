using UnityEngine;

public class Delivery : MonoBehaviour
{
    [Header("Package Settings")]
    [SerializeField] float destroyDelay = 0.5f;
    [SerializeField] Color packageColor = Color.yellow;
    [SerializeField] Color normalColor = Color.white;

    SpriteRenderer spriteRenderer;
    ParticleSystem packageParticles;
    private bool hasPackage = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        packageParticles = GetComponent<ParticleSystem>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Pick up package
        if (collision.CompareTag("Packages") && !hasPackage)
        {
            Debug.Log("Picked up package");
            hasPackage = true;
            spriteRenderer.color = packageColor;
            packageParticles.Play();
            Destroy(collision.gameObject, destroyDelay);
        }

        // Deliver to customer
        if (collision.CompareTag("Dropzone") && hasPackage)
        {
            Debug.Log("Delivered package!");
            hasPackage = false;
            spriteRenderer.color = normalColor;
            packageParticles.Stop();
            Destroy(collision.gameObject);
        }
    }
}