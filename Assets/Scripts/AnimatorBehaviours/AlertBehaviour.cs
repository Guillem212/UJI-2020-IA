using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


//[SharedBetweenAnimators]
public class AlertBehaviour : StateMachineBehaviour
{
    StateControlWaluigi m_stateControl;    

    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AudioManager.instance.SetVolumeSmooth("TensionBreathing", 1f, 1f);
        AudioManager.instance.SetVolumeSmooth("TensionHeartbeat", 0.5f, 0.7f);
        AudioManager.instance.SetVolumeSmooth("AmbientPiano", 0f, 1.2f);
        AudioManager.instance.WaluigiAngrySound();
    }

    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {                        
        if (m_stateControl.m_detectionRatio >= 1)
            animator.SetInteger("State", 2);

        //return to calm
        //StateControlTest.characterState = StateControlTest.States.detected;
        if (m_stateControl.m_detectionRatio <= 0)
            animator.SetInteger("State", 0);
        
    }    

    // OnStateMachineEnter is called when entering a state machine via its Entry Node
    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {        
        if (m_stateControl == null)
            m_stateControl = FindObjectOfType<StateControlWaluigi>();             
    }

    // OnStateMachineExit is called when exiting a state machine via its Exit Node
    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        m_stateControl.OnStateChanged();        
    }
}
