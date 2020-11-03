using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateControlTest : MonoBehaviour
{
    [Range(0f,1f)]
    public float m_detectionRatio = 0f;    

    static Animator anim;    
    
    public enum States { neutral, alert, detected};
    [SerializeField] public States characterState = States.neutral;    

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //print(anim.GetCurrentAnimatorStateInfo(0));

        //head movement = detection ratio
        anim.SetLayerWeight(1, m_detectionRatio);        
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
