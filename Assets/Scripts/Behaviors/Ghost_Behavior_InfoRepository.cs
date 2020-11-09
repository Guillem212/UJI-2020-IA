using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost_Behavior_InfoRepository : MonoBehaviour
{
    [Header("Navigation")]
    //public NavigationAgent navigationAgent;
    public Transform[] waypoints;

    //Private
    private bool playerInRange;

    public bool GetPlayerInRange()
    {
        return playerInRange;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {

        if (other.tag == "Player")
        {
            playerInRange = false;
        }
    }
}
