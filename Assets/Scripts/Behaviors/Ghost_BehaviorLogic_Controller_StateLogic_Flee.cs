using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost_BehaviorLogic_Controller_StateLogic_Flee : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    private Ghost_Behavior_InfoRepository infoRepository;
    private StateControlWaluigi stateControlWaluigi;
    private Transform [] waypoint;
    private Unit navigationAgent;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        infoRepository = animator.gameObject.GetComponent<Ghost_Behavior_InfoRepository>();

        GameObject waluigi = GameObject.FindGameObjectWithTag("Waluigi");
        stateControlWaluigi = waluigi.GetComponent<StateControlWaluigi>();
        infoRepository = animator.gameObject.GetComponent<Ghost_Behavior_InfoRepository>();
        waypoint = infoRepository.waypointFlee;
        navigationAgent = infoRepository.navigationAgent;
        navigationAgent.SetDestination(waypoint[0].position);
        //stateControlWaluigi.EnemieAlertNotification(0.7f);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!infoRepository.GetPlayerInRange() && navigationAgent.finishPath)
        {
            if(navigationAgent.StopPath()) animator.SetTrigger("Patrol");
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
