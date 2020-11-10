using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeySpawner : MonoBehaviour
{
    private Transform[] spawnPoints;
    [SerializeField] private GameObject Key;

    // Start is called before the first frame update
    void Start()
    {
        spawnPoints = gameObject.GetComponentsInChildren<Transform>();
        int r = Random.Range(1, spawnPoints.Length);
        Instantiate(Key, spawnPoints[r]);
    }
}
