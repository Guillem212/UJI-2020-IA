using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost_BehaviorLogic_Controller_StateLogic_Patrolling : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    private Ghost_Behavior_InfoRepository infoRepository;
    private Transform[] waypoints;
    private NavigationAgent navigationAgent;
    int m_CurrentWaypointIndex;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        infoRepository = animator.gameObject.GetComponent<Ghost_Behavior_InfoRepository>();
        waypoints = infoRepository.waypoints;
        //navigationAgent = infoRepository.navigationAgent;
        //navigationAgent.SetDestination(waypoints[0].position);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (infoRepository.GetPlayerInRange())
        {
            animator.SetTrigger("Flee");
        }

        /*if (navigationAgent.GetRemainingDistance() <= navigationAgent.minDistance)
        {
            m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
            navigationAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
        }*/
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
