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
    private bool isRunning = false;

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
        } else
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(currentTime);

            timerText.text = "deliverrd. \n" + string.Format("{0:00}:{1:00}.{2:00}",
                timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds / 10);
        }
        packagesLeftText.text = packages.Count().ToString();
    }


}
