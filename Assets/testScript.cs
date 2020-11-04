using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testScript : MonoBehaviour
{
    public Transform target;
    NavigationAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavigationAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!agent.hasPath)
        {
            agent.SetDestination(target.position);
        }
        agent.MoveAgent();
    }
}
