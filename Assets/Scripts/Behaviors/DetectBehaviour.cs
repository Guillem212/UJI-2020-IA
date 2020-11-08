using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectBehaviour : StateMachineBehaviour
{
    StateControlWaluigi m_stateControl;
    GameObject m_waluigi;
    GameObject m_player;
    GameEnding endGame;
    float distanceToJumpScare = 2f;
    
    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (m_stateControl.m_detectionRatio < 1)
            animator.SetInteger("State", 1);

        //check distance to player
        if (Vector3.Distance(m_waluigi.transform.position, m_player.transform.position) < distanceToJumpScare)
        {            
            endGame.JumpScare();
        }
    }   

    // OnStateMachineEnter is called when entering a state machine via its Entry Node
    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {        
        if (m_stateControl == null)
        {
            m_stateControl = FindObjectOfType<StateControlWaluigi>();
            m_waluigi = m_stateControl.transform.gameObject;
            m_player = GameObject.Find("Player");
            endGame = FindObjectOfType<GameEnding>();
        }

        m_stateControl.m_enemyDestinationReached = false; //NOTE: FOR DEBUG
        AudioManager.instance.SetDetected(true);
        FindObjectOfType<PlayerController>().Run(true);
        m_stateControl.SetDestiny();
    }

    // OnStateMachineExit is called when exiting a state machine via its Exit Node
    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        m_stateControl.OnStateChanged();
        AudioManager.instance.SetDetected(false);
        FindObjectOfType<PlayerController>().Run(false);
    }
}
