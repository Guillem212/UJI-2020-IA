using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaypointPatrol : MonoBehaviour
{
    //public NavigationAgent navigationAgent;
    public Transform[] waypoints;
    public bool df;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        df = true;
    }
    void Update ()
    {
        if(df)
        {
            df = false;
            //navigationAgent.SetDestination(waypoints);
        }

    }
}
