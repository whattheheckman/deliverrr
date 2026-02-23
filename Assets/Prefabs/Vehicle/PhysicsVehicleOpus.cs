using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PhysicsVehicle : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float forwardForce = 20f;
    [SerializeField] private float backwardForce = 10f;
    [SerializeField] private float maxSpeed = 10f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Tilemap Speed Reduction")]
    [SerializeField] private float speedReductionMultiplier = 0.5f;
    [SerializeField] private string groundSortingLayerName = "Ground";

    private Rigidbody2D rb;
    private Camera mainCamera;
    private ParticleSystem exhaustParticles;
    private bool isOnSlowTilemap = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;
        exhaustParticles = GetComponent<ParticleSystem>();
    }

    public void setSpeed(float speed)
    {
        forwardForce = speed;
    }

    public float getSpeed()
    {
        return forwardForce;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Tilemap tilemap = collision.GetComponent<Tilemap>();
        if (tilemap != null)
        {
            TilemapRenderer tilemapRenderer = tilemap.GetComponent<TilemapRenderer>();
            if (tilemapRenderer != null && tilemapRenderer.sortingLayerName == groundSortingLayerName)
            {
                isOnSlowTilemap = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Tilemap tilemap = collision.GetComponent<Tilemap>();
        if (tilemap != null)
        {
            TilemapRenderer tilemapRenderer = tilemap.GetComponent<TilemapRenderer>();
            if (tilemapRenderer != null && tilemapRenderer.sortingLayerName == groundSortingLayerName)
            {
                isOnSlowTilemap = false;
            }
        }
    }

    void Update()
    {
        // Rotate toward mouse cursor
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 direction = (mouseWorldPos - transform.position).normalized;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        float smoothedAngle = Mathf.LerpAngle(rb.rotation, targetAngle, rotationSpeed * Time.deltaTime);
        rb.rotation = smoothedAngle;

        // Particle system based on speed
        float speed = rb.linearVelocity.magnitude;
        var emiss = exhaustParticles.emission;
        emiss.rateOverTime = Mathf.Lerp(3, 20, speed / maxSpeed);
    }

    void FixedUpdate()
    {
        float effectiveForward = isOnSlowTilemap ? forwardForce * speedReductionMultiplier : forwardForce;
        float effectiveBackward = isOnSlowTilemap ? backwardForce * speedReductionMultiplier : backwardForce;

        // Forward force (W)
        if (Keyboard.current.wKey.isPressed)
        {
            rb.AddForce(transform.up * effectiveForward);
        }

        // Backward force (S)
        if (Keyboard.current.sKey.isPressed)
        {
            rb.AddForce(-transform.up * effectiveBackward);
        }

        // Clamp to max speed
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }
}
