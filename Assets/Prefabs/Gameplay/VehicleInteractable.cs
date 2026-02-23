using UnityEngine;
using System.Collections;

public class Interactable : MonoBehaviour
{

    [SerializeField] ParticleSystem unusedParticles;
    [SerializeField] ParticleSystem usedParticles;
    [SerializeField] Color disabledColor = new Color(1f, 1f, 1f, 0.4f);
    [SerializeField] float speedMultiplier = 1.5f;
    [SerializeField] float duration = 5f;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Collider2D pickupCollider;
    private bool used = false;


    void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision);
        if (collision.CompareTag("Player") && !used)
        {
            Debug.Log("it'd be the player!");
            used = true;
            if (unusedParticles != null) { unusedParticles.Stop(); }
            if (usedParticles != null) {
                 usedParticles.transform.rotation = collision.transform.rotation;
                usedParticles.Play(); }
            if (spriteRenderer != null) { spriteRenderer.color *= disabledColor; }

            var playerMovement = collision.GetComponent<VehicleController>();
            if (playerMovement != null)
            {
                StartCoroutine(ApplySpeedBoost(playerMovement));
            }
            pickupCollider.enabled = false;

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
