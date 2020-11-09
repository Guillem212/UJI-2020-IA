using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actions : MonoBehaviour
{
    private InputManager inputs;
    [SerializeField] private LayerMask Battery;

    private RaycastHit m_raycastHit;
    private Ray m_rayOrigin;
    private Outline outlineScript;
    const float rayLenght = 1.4f; //>1 
    [HideInInspector] public bool m_canPickUpBattery = false;
    [SerializeField] GameObject objectToGrab; //if canPickUp then we have the physical object to delete when the action happens
    
    [Space]
    [Header("Debug")]
    [SerializeField] bool d_DebugRays = false;
    public bool d_doCamRaycast = true;

    // Start is called before the first frame update
    void Start()
    {
        inputs = GetComponent<InputManager>();
    }
    
    void FixedUpdate()
    {
        /*if (!d_doCamRaycast) return;
        m_rayOrigin = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));        
        
        if (Physics.Raycast(m_rayOrigin, out m_raycastHit, rayLenght, Battery))
        {
            if (d_DebugRays) Debug.DrawRay(m_rayOrigin.direction, m_raycastHit.point, Color.yellow);

            if (objectToGrab == null) //avoid multiple assignment 
            {
                objectToGrab = m_raycastHit.transform.gameObject; //store object to grab
                outlineScript = m_raycastHit.transform.gameObject.GetComponent<Outline>();
                outlineScript.enabled = true; //outline shader
                m_canPickUpBattery = true; //public variable to check
                print(m_raycastHit.collider.tag + "otro");
            }            
        }
        else
        {
            objectToGrab = null;
            m_canPickUpBattery = false;
            if (outlineScript != null) outlineScript.enabled = false;
        }     */  
    }
}
