using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class Package : MonoBehaviour
{
    private int packageId;
    private SpriteRenderer spriteRenderer;
    private Collider2D myCollider;
    private bool isPickupable = true;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<Collider2D>();
        if (myCollider == null)
        {
            Debug.LogError("missing collider component for package: " + this.name);
        }
        if (spriteRenderer == null)
        {
            Debug.LogError("missing sprite component for package: " + this.name);
        }
    
    }

    public bool packageCanBePickedUp() {return isPickupable;}
    public void setPackageCanBePickedUp(bool canBePickedUp){
        isPickupable = canBePickedUp;
        myCollider.enabled = isPickupable;
        spriteRenderer.enabled = isPickupable;
    }

    public void setPackageID(int incomingId)
    {
        packageId = incomingId;
    }

    public int getPackageID(){ return packageId;}
    
}