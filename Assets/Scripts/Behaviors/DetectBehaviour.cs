using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectBehaviour : StateMachineBehaviour
{
    StateControlWaluigi m_stateControl;

    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_stateControl.m_detectionRatio < 1)
            animator.SetInteger("State", 1);
    }

    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called before OnStateMove is called on any state inside this state machine
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateIK is called before OnStateIK is called on any state inside this state machine
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMachineEnter is called when entering a state machine via its Entry Node
    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {        
        if (m_stateControl == null)
            m_stateControl = FindObjectOfType<StateControlWaluigi>();

        AudioManager.instance.SetDetected(true);
        FindObjectOfType<PlayerController>().Run(true);
    }

    // OnStateMachineExit is called when exiting a state machine via its Exit Node
    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        m_stateControl.OnStateChanged();
        AudioManager.instance.SetDetected(false);
        FindObjectOfType<PlayerController>().Run(false);
    }
}
