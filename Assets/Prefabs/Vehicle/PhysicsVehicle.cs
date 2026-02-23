using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using Unity.Cinemachine;
using static UnityEngine.ParticleSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PhysicsVehicle : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float forwardForce = 10f;
    [SerializeField] private float backwardForce = 5f;
    [SerializeField] private float maxSpeed = 15f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Tilemap Speed Reduction")]
    [SerializeField] private float speedReductionMultiplier = 0.5f; // 0.5 = 50% speed
    [SerializeField] private string groundSortingLayerName = "Ground";

    [Header("Effects")]
    [SerializeField] private ParticleSystem exhaustParticles;

    private Rigidbody2D rb;
    private Camera mainCamera;
    private bool isOnSlowTilemap = false;
    private float defaultForwardForce;

    private EmissionModule particleEmission;
    private Color32 defaultParticleColor;
    private Color32 slowParticleColor = new(67, 67, 67, 240);
    private Color32 fastParticleColor = new(255, 215, 0, 255);

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found! Please add a Rigidbody2D to this GameObject.");
        }

        mainCamera = CinemachineBrain.GetActiveBrain(0).OutputCamera;
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
        }

        defaultForwardForce = forwardForce;

        if (exhaustParticles != null)
        {
            particleEmission = exhaustParticles.emission;
            defaultParticleColor = exhaustParticles.startColor;
        }
        else
        {
            Debug.Log("particle system not found, make sure to assign to VehicleController in editor!");
        }
    }

    public void setSpeed(float speed)
    {
        forwardForce = speed;
        if (exhaustParticles != null)
        {
            if (speed > defaultForwardForce)
            {
                exhaustParticles.startColor = fastParticleColor;
            }
            else if (speed < defaultForwardForce)
            {
                exhaustParticles.startColor = slowParticleColor;
            }
            else
            {
                exhaustParticles.startColor = new Color32(255, 255, 255, 255);
            }
        }
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
        // Update particle emission based on actual velocity
        if (exhaustParticles != null && rb != null)
        {
            float speed = rb.linearVelocity.magnitude;
            particleEmission.rateOverTime = Mathf.Lerp(4.5f, 10f, speed / maxSpeed);
        }

        // Debug line to mouse (visual only, updated every frame)
        if (mainCamera != null && Mouse.current != null && rb != null)
        {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Debug.DrawLine(rb.position, mousePosition, Color.red);
        }
    }

    void FixedUpdate()
    {
        // Handle rotation to follow mouse cursor
        if (mainCamera != null && Mouse.current != null && rb != null)
        {
            Vector3 mousePosition = new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0);
            //Debug.Log($"mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue()) {mainCamera.ScreenToViewportPoint(mousePosition)}");
            mousePosition.z = 0f;

            Vector2 direction = (mousePosition - (Vector3)mainCamera.WorldToScreenPoint(rb.position));
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

            // Smoothly rotate towards target angle
            float newAngle = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, rotationSpeed * 10f * Time.fixedDeltaTime);
            rb.SetRotation(newAngle);
            //Debug.Log($"Rotating to: {targetAngle}, moving towards: {newAngle}, Current: {rb.rotation}");
        }

        // Apply forward force when W is pressed
        if (Keyboard.current != null && Keyboard.current.wKey.isPressed)
        {
            float effectiveForce = isOnSlowTilemap ? forwardForce * speedReductionMultiplier : forwardForce;
            rb.AddForce(transform.up * effectiveForce);
        }

        // Apply backward force when S is pressed
        if (Keyboard.current != null && Keyboard.current.sKey.isPressed)
        {
            float effectiveForce = isOnSlowTilemap ? backwardForce * speedReductionMultiplier : backwardForce;
            rb.AddForce(-transform.up * effectiveForce);
        }

        // Clamp velocity to max speed
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }
}