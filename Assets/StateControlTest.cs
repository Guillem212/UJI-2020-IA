using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateControlTest : MonoBehaviour
{
    NavigationAgent navAgent;
    public Transform destination;

    [Range(0f,1f)]
    public float m_detectionRatio = 0f;          

    static Animator anim;    
    
    public enum States { neutral, alert, detected};
    [SerializeField] public States characterState = States.neutral;

    [Space]
    [Header("Detection debug")]
    [SerializeField] bool m_watchingPlayer = false;
    [SerializeField] float m_detectionmultiplierUp = 0.35f;
    [SerializeField] float m_detectionmultiplierDown = 0.05f;

    private bool needToAssingStuff = false;
    

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        m_detectionRatio = 0f;
        navAgent = GetComponent<NavigationAgent>();
        navAgent.SetDestination(destination.position);

        m_detectionmultiplierUp = 0.35f; //detection up multiplier ratio
        m_detectionmultiplierDown = 0.05f; //detection down multiplier ratio
    }

    // Update is called once per frame
    void Update()
    {
        //print(anim.GetCurrentAnimatorStateInfo(0));           

        navAgent.MoveAgent();

        UpdateDetectionRatio();                  

        anim.SetLayerWeight(1, m_detectionRatio); //head movement
    }    

    private void UpdateDetectionRatio()
    {
        if (needToAssingStuff) return;

        if (m_watchingPlayer)
        {
            if (m_detectionRatio < 1f)
            {
                if (m_detectionRatio == 0f) print("visto");//sound                
                m_detectionRatio += m_detectionmultiplierUp * Time.deltaTime;
            }
            else
                m_detectionRatio = 1f;
        }

        else
        {
            if (m_detectionRatio > 0f)
                m_detectionRatio -= m_detectionmultiplierDown * Time.deltaTime;
            else
            {
                m_detectionRatio = 0f;
            }
        }
    }

    public void OnStateChanged()
    {
        switch (anim.GetInteger("State"))
        {
            case 0:
                {
                    characterState = States.neutral;
                    m_detectionRatio = 0f;
                    break;
                }
            case 1:
                {
                    characterState = States.alert;                    
                    break;
                }
            case 2:
                {
                    characterState = States.detected;
                    m_detectionRatio = 1f;
                    break;
                }
            default: { Debug.LogError("State animator input mismatch"); break; }
               
        }
    }
}
