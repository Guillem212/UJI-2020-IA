using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaypointPatrol : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public NavigationAgent navigationAgent;
    public Transform[] waypoints;

    int m_CurrentWaypointIndex;

    void Start ()
    {
        navigationAgent.SetDestination (waypoints[0].position);
    }

    void Update ()
    {
        if(navigationAgent.GetRemainingDistance() <= navigationAgent.minDistance)
        {
            m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
            navigationAgent.SetDestination (waypoints[m_CurrentWaypointIndex].position);
        }
    }
}
