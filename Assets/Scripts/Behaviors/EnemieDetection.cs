using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemieDetection : MonoBehaviour
{
    public Transform player;
    bool detected;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == player)
        {
            Debug.Log("Te veo!");
            detected = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform == player)
        {
            Debug.Log("No te veo!");
            detected = false;
        }
    }
}
