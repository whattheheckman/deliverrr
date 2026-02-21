using UnityEngine;
using System.Collections;

public class Interactable : MonoBehaviour
{

    [SerializeField] Color disabledColor = new Color(1f, 1f,1f, 0.5f);
    [SerializeField] float speedMultiplier = 1.5f;
    [SerializeField] float duration = 5f;


    SpriteRenderer spriteRenderer;
    ParticleSystem particles;
    private bool used = false;

    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        particles = GetComponent<ParticleSystem>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision);
        if (collision.CompareTag("Player") && !used) 
        {
            Debug.Log("it'd be the player!");
            used = true;
            if (particles != null) {particles.Play();}
            if (spriteRenderer != null) {spriteRenderer.color *= disabledColor;}

            var playerMovement = collision.GetComponent<VehicleController>();
            if (playerMovement != null)
            {
                StartCoroutine(ApplySpeedBoost(playerMovement));
            }
        }
    }

    IEnumerator ApplySpeedBoost(VehicleController player)
    {
        float originalSpeed = player.getSpeed();
        player.setSpeed(originalSpeed *= speedMultiplier);

        yield return new WaitForSeconds(duration);

        player.setSpeed(originalSpeed);
    }

}
