using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateControlWaluigi : MonoBehaviour
{
    Unit navAgent;
    public Transform destination;
    GameObject m_player;
    Lantern c_playerLantern;
    ObserverWaluigi c_observer;
    PPEffects c_postPorcessing;

    [Range(0f,1f)]
    public float m_detectionRatio = 0f;          

    static Animator anim;    
    
    public enum States { neutral, alert, detected};
    [SerializeField] public States characterState = States.neutral;

    [Space]
    [Header("Detection debug")]
    public bool m_watchingPlayer = false;
    public bool m_detectingLantern = false;        

    private float m_detectionmultiplierUp = 0.35f;//detection up multiplier ratio
    const float m_detectionmultiplierDown = 0.05f;//detection down multiplier ratio
    private float m_detectionRangeLimit = 6f;
    private float m_detectionMultiplierLantern = 0.35f; //0.35
    private float m_detectionMultiplierWithoutLantern = 0.05f;

    private float distanceToJumpScare = 1f;
    private bool jumpScareActivated = false;

    public bool m_enemyDestinationReached = true;
    [SerializeField] private bool m_normalPathActivated = false;    
    private bool m_changedPathType = false; //initialize the dynamic path vars for first time    
    [SerializeField] private bool m_dynamicPathActivated = false;        

    private bool m_canModifyDetection = false;
    private float m_cooldownCounter = 0f;
    private float m_cooldownCounterMax = 2.5f;
    
    private bool needsToAssingStuff = false;

    private bool initialized = false;
    [Space]
    [Header("Debug")]
    [SerializeField] bool d_debugVars = false;

    // Start is called before the first frame update
    void Start()
    {
        m_player = GameObject.Find("Player");
        c_observer = GetComponentInChildren<ObserverWaluigi>();
        c_postPorcessing = FindObjectOfType<PPEffects>();
        c_playerLantern = m_player.GetComponent<Lantern>();
        anim = GetComponent<Animator>();
        m_detectionRatio = 0f;
        navAgent = GetComponent<Unit>();

        m_canModifyDetection = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (needsToAssingStuff) return;

        //check distance to player
        if (Vector3.Distance(transform.position, m_player.transform.position) < distanceToJumpScare && !jumpScareActivated)
        {
            FindObjectOfType<GameEnding>().JumpScare();
            jumpScareActivated = true;
        }

        if (m_watchingPlayer || m_detectingLantern)
        {
            c_postPorcessing.SetAlertFeedbackPP(true);
            m_cooldownCounter = m_cooldownCounterMax;
        }
        else
        {
            c_postPorcessing.SetAlertFeedbackPP(false);
        }              

        //dinamico
        if ((!m_canModifyDetection && m_dynamicPathActivated) || m_watchingPlayer || m_detectingLantern)
        {
            SetDinamicDestiny();            
        }
       
        //cambio a normal
        /*if (m_dynamicPathActivated && !m_watchingPlayer && m_canModifyDetection)
        {
            m_dynamicPathActivated = false;
            m_changedPathType = false;
            //SetDestiny();
        }   */    

        //llegada en normal
        if (m_normalPathActivated && !m_dynamicPathActivated && navAgent.finishPath)
        {
            m_normalPathActivated = false;
            m_enemyDestinationReached = true;
            m_changedPathType = false;
        }

        UpdateDetectionRatio();                  
        anim.SetLayerWeight(1, m_detectionRatio); //head movement
    }

    public void SetDestiny()
    {
        navAgent.SetDestination(m_player.transform.position);
        m_normalPathActivated = true;
        m_enemyDestinationReached = false;
    }
    public void SetDinamicDestiny()
    {        
        if (!m_changedPathType)
        {
            m_changedPathType = true;

            print("counter");
            m_canModifyDetection = false;
            m_dynamicPathActivated = true;
            m_normalPathActivated = false;
            m_enemyDestinationReached = false;
        }
        navAgent.SetDynamicDestination(m_player.transform);
    }    


    private void UpdateDetectionRatio()
    {
        m_detectingLantern = false;

        if (m_watchingPlayer)
        {            
            if (m_detectionRatio >= 1f) m_detectionRatio = 1f;
            else
            {                
                m_detectionmultiplierUp = (c_playerLantern.m_usingLantern) ? m_detectionMultiplierLantern : m_detectionMultiplierWithoutLantern;
                //print(Vector3.Distance(transform.position, c_player.transform.position));                
                m_detectionRatio += (m_detectionmultiplierUp + (m_detectionRangeLimit - Vector3.Distance(transform.position, m_player.transform.position)) / 100f) * Time.deltaTime;
            }            
        }

        else
        {
            //if we are using lantern toward him
            if (c_playerLantern.m_usingLantern && Vector3.Distance(transform.position, m_player.transform.position) < m_detectionRangeLimit)
            {
                if (RaycastFromPlayerToWaluigi() && m_detectionRatio < 1f) //RAYCAST DEL JUGADOR A WALUIGI
                {
                    m_detectionRatio += (m_detectionMultiplierLantern + m_detectionRangeLimit / 100f) * Time.deltaTime;
                    m_detectingLantern = true;
                }
            }

            if (!m_canModifyDetection && m_dynamicPathActivated)
            {
                m_cooldownCounter -= Time.deltaTime;
                if (m_cooldownCounter <= 0f)
                {
                    m_cooldownCounter = m_cooldownCounterMax;
                    m_canModifyDetection = true;
                    m_enemyDestinationReached = true;
                    m_dynamicPathActivated = false;
                    m_changedPathType = false;
                    print("eh familia que he lelgado y esta todo vienb de berda muchas gracias");
                }
            }

            else if (m_detectionRatio > 0f && m_enemyDestinationReached && m_canModifyDetection)
            {
                m_detectionRatio -= m_detectionmultiplierDown * Time.deltaTime;                
            }
        }  
        
        //if (m_detectingLantern || m_watchingPlayer) 
    }    

    bool RaycastFromPlayerToWaluigi()
    {
        //raycast check
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));        
        RaycastHit raycastHit;
        //Debug.DrawRay(ray.origin, ray.direction, Color.yellow, 0.1f);
        if (Physics.Raycast(ray, out raycastHit, 100f))
        {
            //print(raycastHit.transform);            
            return (raycastHit.transform.CompareTag("Waluigi"));
        }
        return false;
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
                    navAgent.speed = 2f;
                    break;
                }
            case 1:
                {
                    characterState = States.alert;                    
                    navAgent.speed = 3f;
                    break;
                }
            case 2:
                {
                    characterState = States.detected;
                    m_detectionRatio = 1f;
                    navAgent.speed = 10f;
                    break;
                }
            default: { Debug.LogError("State animator input mismatch"); break; }
               
        }
    }

    void f_AnimationEventKillWaluigi()
    {
        this.gameObject.SetActive(false);
    }

    //Funcion que llaman los enemigos al ver al jugador
    public void EnemieAlertNotification(float alertRatio)
    {
        m_detectionRatio = alertRatio;
    }


    private void OnGUI()
    {
        //GUI.contentColor = Color.black;
        if (d_debugVars)
        {
            GUI.Label(new Rect(10, 20, 1000, 20), "counter: " + m_cooldownCounter);
            GUI.Label(new Rect(10, 40, 1000, 20), "dynamic: " + m_dynamicPathActivated);
            GUI.Label(new Rect(10, 60, 1000, 20), "normal: " + m_normalPathActivated);
            GUI.Label(new Rect(10, 80, 1000, 20), "detecting: " + (m_watchingPlayer || m_detectingLantern));                      
            GUI.Label(new Rect(10, 100, 1000, 20), "destination reached: " + m_enemyDestinationReached);                      
            GUI.Label(new Rect(10, 120, 1000, 20), "alert: " + m_detectionRatio);                      
        }             
    }
}
