using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateControlWaluigi : MonoBehaviour
{
    //NavigationAgent navAgent;
    public Transform destination;

    [Range(0f,1f)]
    public float m_detectionRatio = 0f;          

    static Animator anim;    
    
    public enum States { neutral, alert, detected};
    [SerializeField] public States characterState = States.neutral;

    [Space]
    [Header("Detection debug")]
    [SerializeField] bool m_watchingPlayer = false;
    const float m_detectionmultiplierUp = 0.35f;//detection up multiplier ratio
    const float m_detectionmultiplierDown = 0.05f;//detection down multiplier ratio

    public bool m_enemyDestinationReached = true;    

    private bool needsToAssingStuff = false;    
    

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        m_detectionRatio = 0f;
        //navAgent = GetComponent<NavigationAgent>();
        //navAgent.SetDestination(destination.position);               
    }

    // Update is called once per frame
    void Update()
    {
        if (needsToAssingStuff) return;        

        //print(anim.GetCurrentAnimatorStateInfo(0));           

        //navAgent.MoveAgent();

        UpdateDetectionRatio();                  

        anim.SetLayerWeight(1, m_detectionRatio); //head movement

        //m_watchingPlayer = Input.GetKey(KeyCode.R);
    }    

    private void UpdateDetectionRatio()
    {        
        if (m_watchingPlayer)
        {            
            if (m_detectionRatio >= 1f) m_detectionRatio = 1f;
            else
            {
                /*if (m_detectionRatio <= 0f) //first time detection
                {
                    //sound
                    AudioManager.instance.SetVolumeSmooth("TensionBreathing", 1f, 1f);
                    AudioManager.instance.SetVolumeSmooth("TensionHeartbeat", 0.5f, 0.7f);
                    AudioManager.instance.SetVolumeSmooth("AmbientPiano", 0f, 1.2f);
                    AudioManager.instance.WaluigiAngrySound();
                }*/
                m_detectionRatio += m_detectionmultiplierUp * Time.deltaTime;
            }

            //set destination to player

            //m_enemyDestinationReached = false;
        }

        else
        {                                        
            if (m_detectionRatio > 0f && m_enemyDestinationReached)
            {
                m_detectionRatio -= m_detectionmultiplierDown * Time.deltaTime;
                if (m_detectionRatio <= 0f) //back to calm
                {
                    //sound
                    AudioManager.instance.SetVolumeSmooth("TensionBreathing", 0f, 0.5f);
                    AudioManager.instance.SetVolumeSmooth("TensionHeartbeat", 0f, 0.6f);
                    AudioManager.instance.SetVolumeSmooth("AmbientPiano", 0.089f, 0.2f);
                    m_detectionRatio = 0f;
                }
            }
        }        
    }    

    /// <summary>
    /// Called from animation behaviours
    /// </summary>
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
