using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutralBehaviour : StateMachineBehaviour
{
    StateControlWaluigi m_stateControl;
    bool firstEntry = true;

    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {        
        if (m_stateControl.m_detectionRatio > 0)
            animator.SetInteger("State", 1);        
    }    

    // OnStateMachineEnter is called when entering a state machine via its Entry Node
    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        if (!firstEntry)
        {
            //sound
            AudioManager.instance.SetVolumeSmooth("TensionBreathing", 0f, 0.5f);
            AudioManager.instance.SetVolumeSmooth("TensionHeartbeat", 0f, 0.6f);
            AudioManager.instance.SetVolumeSmooth("AmbientPiano", 0.089f, 0.2f);
            m_stateControl.m_detectionRatio = 0f;
        }
        if (m_stateControl == null)
        {
            m_stateControl = FindObjectOfType<StateControlWaluigi>();
            firstEntry = false;
        }
        
    }

    // OnStateMachineExit is called when exiting a state machine via its Exit Node
    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        m_stateControl.OnStateChanged();
    }
}
