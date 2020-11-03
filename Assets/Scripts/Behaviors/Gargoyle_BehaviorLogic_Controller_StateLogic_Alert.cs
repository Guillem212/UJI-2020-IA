using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gargoyle_BehaviorLogic_Controller_StateLogic_Alert : StateMachineBehaviour
{
    private Gargoyle_Behavior_InfoRepository infoRepository;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        infoRepository = animator.gameObject.GetComponent<Gargoyle_Behavior_InfoRepository>();
        infoRepository.gargoyle.GetComponent<Renderer>().material = infoRepository.lightAlert;
        Debug.Log("Alerta!!");
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject player = GameObject.FindWithTag("Player");
        float distance = Vector3.Distance(player.transform.position, animator.gameObject.transform.position);
        if (distance >= 6)
        {
            animator.SetTrigger("Patrol");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
