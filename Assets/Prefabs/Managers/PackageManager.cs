using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class PackageManager : MonoBehaviour
{
    [Header("Packages")]
    [SerializeField] private GameObject[] packages;
    [Header("Dropzones")]
    [SerializeField] private GameObject[] dropzones;
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI packagesLeftText;
    [SerializeField] 


    private float currentTime;
    private int packagesRemainingCount = 0;

    void Start()
    {
        if (dropzones.Count() != packages.Count())
        {
            Debug.LogError("Package and drop zone count doesn't match! early returning from Start()" );
            return;
        }
        for (int i = 0; i < packages.Count(); i++)
        {
        
            dropzones[i].GetComponent<Package>().setPackageCanBePickedUp(false);
            dropzones[i].GetComponent<Package>().setPackageID(i);
            packages[i].GetComponent<Package>().setPackageCanBePickedUp(true);
            packages[i].GetComponent<Package>().setPackageID(i);
        }
        currentTime = 0f;
        timerText.text = "00:00.00";
        packagesRemainingCount = dropzones.Count();
    }

    public void pickupPackage(int packageId)
    {   
        packages[packageId].GetComponent<Package>().setPackageCanBePickedUp(false);
        dropzones[packageId].GetComponent<Package>().setPackageCanBePickedUp(true);
    }

    public void dropoffPackage(int dropzoneID)
    {
        dropzones[dropzoneID].GetComponent<Package>().setPackageCanBePickedUp(false);
        packagesRemainingCount-= 1;
    }

    void Update()
    {
        
        if (packagesRemainingCount > 0)
        {
            currentTime += Time.deltaTime;
            TimeSpan timeSpan = TimeSpan.FromSeconds(currentTime);
            timerText.text = string.Format("{0:00}:{1:00}.{2:00}",
                timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);
            packagesLeftText.text = "Packages left: \n" + packagesRemainingCount.ToString();
        } else
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(currentTime);
            packagesLeftText.text = "Packages left: \n 0";
            timerText.text = "deliverrd. \n" + string.Format("{0:00}:{1:00}.{2:00}",
                timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);
            //Destroy(this, Time.deltaTime * 3f);
        }
        

    }


}
