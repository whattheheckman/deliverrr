using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PackageManager : MonoBehaviour
{
    [Header("Packages")]
    [SerializeField] private GameObject packageInstance;
    [SerializeField] private Transform[] spawnLocations;
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI packagesLeftText;

    private 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Transform spawnLocation in spawnLocations)
        {
            if (spawnLocation != null)
            {
                continue;
            }
            else
            {
                Instantiate(packageInstance, spawnLocation.position, spawnLocation.rotation);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
