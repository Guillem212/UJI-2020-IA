using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateControlWaluigi : MonoBehaviour
{
    NavigationAgent navAgent;
    public Transform destination;
    GameObject c_player;
    Lantern c_playerLantern;
    ObserverWaluigi c_observer;
    PPEffects pp;

    [Range(0f,1f)]
    public float m_detectionRatio = 0f;          

    static Animator anim;    
    
    public enum States { neutral, alert, detected};
    [SerializeField] public States characterState = States.neutral;

    [Space]
    [Header("Detection debug")]
    public bool m_watchingPlayer = false;
    public bool m_detectingLantern = false;
    float m_detectionmultiplierUp = 0.35f;//detection up multiplier ratio
    const float m_detectionmultiplierDown = 0.05f;//detection down multiplier ratio
    float m_detectionRangeLimit = 6f;
    float m_detectionMultiplierLantern = 0.35f;
    float m_detectionMultiplierWithoutLantern = 0.05f;    

    public bool m_enemyDestinationReached = true;    

    private bool needsToAssingStuff = false;

    private bool initialized = false;

    // Start is called before the first frame update
    void Start()
    {
        c_player = GameObject.Find("Player");
        c_observer = GetComponentInChildren<ObserverWaluigi>();
        pp = FindObjectOfType<PPEffects>();
        c_playerLantern = c_player.GetComponent<Lantern>();
        anim = GetComponent<Animator>();
        m_detectionRatio = 0f;
        navAgent = GetComponent<NavigationAgent>();
        StartCoroutine(initialCooldown());        
    }

    // Update is called once per frame
    void Update()
    {
        if (needsToAssingStuff) return;        

        /*if (!initialized)
        {
            navAgent.SetDestination(destination.position);               
            initialized = true;
        }*/

        //print(anim.GetCurrentAnimatorStateInfo(0));           

        if (initialized && characterState == States.detected)
        {            
            navAgent.MoveAgent();
        }

        UpdateDetectionRatio();                  

        anim.SetLayerWeight(1, m_detectionRatio); //head movement

        //m_watchingPlayer = Input.GetKey(KeyCode.R);
    }    

    IEnumerator initialCooldown()
    {
        yield return new WaitForSeconds(1f);
        //navAgent.SetDestination(destination.position);
        initialized = true;
        yield return null;
    }

    public void SetDestiny()
    {
        navAgent.SetDestination(destination.position);
    }


    private void UpdateDetectionRatio()
    {
        m_detectingLantern = false;

        if (m_watchingPlayer)
        {            
            if (m_detectionRatio >= 1f) m_detectionRatio = 1f;
            else
            {
                //depending on some parameters
                m_detectionmultiplierUp = (c_playerLantern.m_usingLantern) ? m_detectionMultiplierLantern : m_detectionMultiplierWithoutLantern;
                //print(Vector3.Distance(transform.position, c_player.transform.position));                
                m_detectionRatio += (m_detectionmultiplierUp + (m_detectionRangeLimit - Vector3.Distance(transform.position, c_player.transform.position)) / 100f) * Time.deltaTime;
            }
            //set destination to player
            //m_enemyDestinationReached = false;
        }

        else
        {
            //if we are using lantern toward him
            if (c_playerLantern.m_usingLantern && Vector3.Distance(transform.position, c_player.transform.position) < m_detectionRangeLimit)
            {
                if (RaycastFromPlayerToWaluigi() && m_detectionRatio < 1f) //RAYCAST DEL JUGADOR A WALUIGI
                {
                    m_detectionRatio += (m_detectionMultiplierLantern + m_detectionRangeLimit / 100f) * Time.deltaTime;
                    m_detectingLantern = true;
                }
            }
            if (m_detectionRatio > 0f && m_enemyDestinationReached)
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

    void f_AnimationEventKillWaluigi()
    {
        this.gameObject.SetActive(false);
    }

    //Funcion que llaman los enemigos al ver al jugador
    public void EnemieAlertNotification(float alertRatio)
    {
        m_detectionRatio = alertRatio;
    }
}
