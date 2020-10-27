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
    public bool hasPath;
    public bool isStopped;
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
    }

    public void SetDestination(Vector3 targetPosition){
        path = pathFinding.CalculatePath(transform.position, targetPosition);
        if (path != null){
            hasPath = true;
        }
        else{
            hasPath = false;
        }
    }

    private void Update() {
        if(!hasPath)
            SetDestination(target.position);
        Move();
        
    }

    public void Move(){
        if(GetRemainingDistance() <= minDistance){
            setNextPosition();
        }
        Vector3 targetPosition = returning? path[0].position : path[path.Count - 1].position; 
        Vector3 playerDistance = targetPosition - transform.position;
        Vector3 playerDistanceToNextNode = (Vector3)path[nextNode].position - transform.position;
        Vector3 desiredVelocity = playerDistanceToNextNode.normalized * maxSpeed;
        Vector3 steering = desiredVelocity - velocity;

        velocity += steering * Time.deltaTime;

        float slowDownFactor = Mathf.Clamp01(playerDistance.magnitude / slowdownDistance); 
        velocity *= slowDownFactor;

        transform.position += velocity * Time.deltaTime;

    }

    private void setNextPosition(){
        if(nextNode < path.Count - 1 && !returning)
            nextNode++;
        else if(nextNode > 0){
            returning = true;
            nextNode--;
        }
        else{
            returning = false;
            nextNode++;
        }
    }

    public float GetRemainingDistance(){
        float difX = Mathf.Pow(path[nextNode].position.x - transform.position.x, 2);
        float difZ = Mathf.Pow(path[nextNode].position.z - transform.position.z, 2);
        remainingDistance = Mathf.Sqrt(difX + difZ);
        return remainingDistance;
    }

    public void ResetPath(){
        path.Clear();
        hasPath = false;
    }


}
