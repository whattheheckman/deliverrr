using UnityEngine;

public class Package : MonoBehaviour
{
    private int packageId;
    private SpriteRenderer spriteRenderer;
    private Collider2D myCollider;
    private bool isPickupable = false;

    void OnStart()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<Collider2D>();
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