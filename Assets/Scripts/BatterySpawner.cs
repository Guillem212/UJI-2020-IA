using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatterySpawner : MonoBehaviour
{
    //Batteries
    private Transform[] spawnPoints;
    [SerializeField] private GameObject Battery;
    [Header("Must be equal or lower than the spawnPoints")]
    [SerializeField] private int num_of_batteries;
    private List<int> spawned;

    // Start is called before the first frame update
    void Start()
    {
        spawned = new List<int>();
        spawnPoints = gameObject.GetComponentsInChildren<Transform>();
        for(int i = 0; i < num_of_batteries; i++)
        {
            int r = Random.Range(1, spawnPoints.Length);
            if (!spawned.Contains(r))
            {
                Instantiate(Battery, spawnPoints[r]); 
                spawned.Add(r);
            }
            else i--;
        }
    }
}
