using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class NavigationAgent : MonoBehaviour
{
    //---------------------------------
    //Public variables
    //---------------------------------
    public float maxSpeed;
    public float minDistance;
    [HideInInspector] public bool hasPath;
    [HideInInspector] public bool isStopped;
    public List<PathNode> path;

    public Transform target;

    //---------------------------------
    //Private variables
    //---------------------------------
    private float slowdownDistance = 1f;
    private Vector3 velocity = Vector3.zero;
    private PathFinding pathFinding;
    private float remainingDistance;
    private int nextNode;
    private bool returning;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start(){
        pathFinding = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PathFinding>();
        nextNode = 0;
        returning = false;
        hasPath = false;
    }

    public void SetDestination(Vector3 targetPosition){
        if(path == null){
            path = pathFinding.CalculatePath(transform.position, targetPosition);
            hasPath = true;
        }
        else{
            ResetPath();
            path = pathFinding.CalculatePath(transform.position, targetPosition);
            hasPath = true;
        }
    }

    private Vector3 targetPosition;
    private Vector3 playerDistance;
    private Vector3 playerDistanceToNextNode;
    private Vector3 desiredVelocity; 
    private Vector3 steering;
    public void MoveAgent(){
        if(hasPath){
            if(nextNode == path.Count - 1 && GetRemainingDistance() <= minDistance){
            isStopped = true;
            ResetPath();
            return;
            }
            else{
                isStopped = false;
            }

            if(GetRemainingDistance() <= minDistance){
                setNextPosition();
            }
            targetPosition = returning ? path[0].position : path[path.Count - 1].position; 
            playerDistance = targetPosition - transform.position;
            playerDistanceToNextNode = (Vector3)path[nextNode].position - transform.position;
            desiredVelocity = playerDistanceToNextNode.normalized * maxSpeed;
            steering = desiredVelocity - velocity;

            velocity += steering * Time.deltaTime;

            float slowDownFactor = Mathf.Clamp01(playerDistance.magnitude / slowdownDistance); 
            velocity *= slowDownFactor;

            transform.position += velocity * Time.deltaTime;
        }
    }

    private void setNextPosition(){
        if(nextNode < path.Count - 1)
            nextNode++;
    }

    private float GetRemainingDistance(){
        float difX = Mathf.Pow(path[nextNode].position.x - transform.position.x, 2);
        float difZ = Mathf.Pow(path[nextNode].position.z - transform.position.z, 2);
        remainingDistance = Mathf.Sqrt(difX + difZ);
        return remainingDistance;
    }

    public float RemainingDistance(){
        if(hasPath){
            float difX = Mathf.Pow(path[path.Count - 1].position.x - transform.position.x, 2);
            float difZ = Mathf.Pow(path[path.Count - 1].position.z - transform.position.z, 2);
            return Mathf.Sqrt(difX + difZ);
        }
    }

    public void ResetPath(){
        path.Clear();
        hasPath = false;
    }


}
