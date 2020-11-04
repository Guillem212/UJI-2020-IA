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

    //---------------------------------
    //Private variables
    //---------------------------------
    private float slowdownDistance = 1f;
    private Vector3 velocity = Vector3.zero;
    private PathFinding pathFinding;
    private float remainingDistance;
    private int nextNode;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    /// 
    void Start(){
        pathFinding = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PathFinding>();
        nextNode = 0;
        hasPath = false;
    }

    public void SetDestination(Vector3 tp){
        if(path == null){
            path = pathFinding.CalculatePath(transform.position, tp);
            hasPath = true;
            nextNode = 0;
        }
        else{
            ResetPath();
            path = pathFinding.CalculatePath(transform.position, tp);
            hasPath = true;
        }
    }

    private Vector3 m_targetPosition;
    private float playerDistance;
    private Vector3 playerDistanceToNextNode;
    private Vector3 desiredVelocity; 
    private Vector3 steering;
    public void MoveAgent(){
        if(hasPath){
            /*if(nextNode == path.Count - 1 && GetRemainingDistance() <= minDistance){
            isStopped = true;
            ResetPath();
            return;
            }
            else{
                isStopped = false;
            }*/

            if(GetRemainingDistance() <= minDistance){
                setNextPosition();
            }
            m_targetPosition = path[path.Count - 1].position; 
            playerDistance = path.Count * Grid.nodeDiameter;
            playerDistanceToNextNode = (Vector3)path[nextNode].position - transform.position;
            desiredVelocity = playerDistanceToNextNode.normalized * maxSpeed;
            steering = desiredVelocity - velocity;

            velocity += steering * Time.deltaTime;

            float slowDownFactor = Mathf.Clamp01(playerDistance / slowdownDistance); 
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
        return -1f;
    }

    public void ResetPath(){
        path.Clear();
        hasPath = false;
    }


}
