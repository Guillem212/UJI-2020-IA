using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObserverWaluigi : MonoBehaviour
{    
    GameObject m_player;
    Transform m_playerTrans;
    StateControlWaluigi c_stateControl;

    public bool m_IsPlayerInTrigger;    

    private void Start()
    {
        m_player = GameObject.Find("Player");
        m_playerTrans = m_player.transform;
        c_stateControl = transform.parent.GetComponent<StateControlWaluigi>();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.transform == m_playerTrans)
        {
            m_IsPlayerInTrigger = true;
        }
        
    }    

    void OnTriggerExit(Collider other)
    {
        if (other.transform == m_playerTrans)
        {
            m_IsPlayerInTrigger = false;
        }
    }

    void Update()
    {        
        if (m_IsPlayerInTrigger)
        {
            //raycast check
            if (RaycastToPlayer())
            {                
                c_stateControl.m_watchingPlayer = true;
            }
            else
            {                
                c_stateControl.m_watchingPlayer = false;
            }             
        }

        else { c_stateControl.m_watchingPlayer = false; }
    }   
    
    public bool RaycastToPlayer()
    {
        //raycast check
        Vector3 direction = m_playerTrans.position - transform.parent.position;
        Ray ray = new Ray(transform.parent.position - Vector3.forward * 0.3f, direction);
        RaycastHit raycastHit;
        Debug.DrawRay(ray.origin, ray.direction, Color.yellow, 0.1f);        
        return Physics.Raycast(ray, out raycastHit, 100f) && (raycastHit.transform == m_playerTrans); 
    }
}
