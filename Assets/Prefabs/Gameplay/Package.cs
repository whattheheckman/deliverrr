using UnityEngine;

public class Package : MonoBehaviour
{
    private int packageId;
    

    public void setPackageID(int incomingId)
    {
        packageId = incomingId;
    }

    public int getPackageID(){ return packageId;}
    
}