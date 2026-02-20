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


    private float currentTime;

    void Start()
    {
        foreach (var zone in dropzones)
        {
            zone.SetActive(false);
        }
        currentTime = 0f;
        timerText.text = "00:00.00";
    }

    void Update()
    {
        if (dropzones.Count() != 0)
        {
            currentTime += Time.deltaTime;
            TimeSpan timeSpan = TimeSpan.FromSeconds(currentTime);
            timerText.text = string.Format("{0:00}:{1:00}.{2:00}",
                timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);
            packagesLeftText.text = "Packages left: \n" + packages.Count().ToString();
            for (int i = 0; i < dropzones.Count(); i++)
            {
                if (packages[i].gameObject == null)
                {
                    dropzones[i].SetActive(true);
                }
            }
        } else
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(currentTime);
            packagesLeftText.text = "Packages left: \n 0";
            timerText.text = "deliverrd. \n" + string.Format("{0:00}:{1:00}.{2:00}",
                timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);
            Destroy(this, Time.deltaTime * 3f);
        }
        

    }


}
