using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;

public class NavigationAgent : MonoBehaviour
{
    //---------------------------------
    //Public variables
    //---------------------------------
    public float maxSpeed;
    public float rotationSpeed;
    public float minDistance;

    public LayerMask obstacleLayer;
    [HideInInspector] public bool hasPath;
    [HideInInspector] public bool isStopped;
    public List<PathNode> path;
    

    //---------------------------------
    //Private variables
    //---------------------------------
    private float slowdownDistance = 5f;
    private Vector3 velocity = Vector3.zero;
    private PathFinding pathFinding;
    private float remainingDistance;
    private int actualNode;
    private Vector3 playerDistanceToactualNode;
    private Vector3 desiredVelocity; 
    private Vector3 steering;


    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start(){
        pathFinding = GameObject.FindGameObjectWithTag("GameManager").GetComponent<PathFinding>();
        actualNode = 0;
        hasPath = false;
        isStopped = true;
    }

    /// <summary>
    /// Set the destination point in the navMesh by the given position, be careful and do not put the target outside the navmesh.
    /// Never asign de destination per frame, or will consume the memory and crash.
    /// </summary>
    public void SetDestination(Vector3 tp){
        if(path == null){
            path = pathFinding.CalculatePath(transform.position, tp);
            hasPath = true;
            actualNode = 0;
        }
        else{
            ResetPath();
            path = pathFinding.CalculatePath(transform.position, tp);
            hasPath = true;
            actualNode = 0;
        }
    }

    /// <summary>
    /// Move towards the destination through a given path.
    /// </summary>
    public void MoveAgent(){
        if(hasPath){
            if(GetRemainingDistance() < minDistance){
                if(actualNode < path.Count - 1)
                    actualNode++;
                else{
                    path.Clear();
                    hasPath = false;
                    actualNode = 0;
                    isStopped = true;
                    return;
                }
            }
            if(isStopped){
                isStopped = false;
            }

            //Distancia manhatan total por hacer xddddd
            playerDistanceToactualNode = (Vector3)path[actualNode].position - transform.position;
            desiredVelocity = playerDistanceToactualNode.normalized * maxSpeed;
            steering = desiredVelocity - velocity;

            velocity += steering * Time.deltaTime;

            float slowDownFactor = Mathf.Clamp01(ManhattanDistance() / slowdownDistance); 
            velocity *= slowDownFactor;
            velocity = new Vector3(velocity.x, 0, velocity.z);

            WhiskersDetection();

            Quaternion rotation = Quaternion.LookRotation(velocity, Vector3.up);
            transform.rotation = rotation;

            transform.position += velocity * Time.deltaTime;
        }
    }

    /// <summary>
    /// Calculate the remaining distance to the target position.
    /// </summary>
    /// <returns>Returns a float with the remaining distance.</returns>
    public float RemainingDistance(){
        if(hasPath){
            float aux = Dist((Vector3)path[actualNode + 1].position, transform.position);
            for (int i = actualNode  + 1; i < path.Count - 2; i++)
            {
                aux += Dist((Vector3)path[i].position , (Vector3)path[i + 1].position);
            }
            return aux;
        }
        return -1;
    }

    /// <summary>
    /// Clear the path that previously have been calculated.
    /// </summary>
    public void ResetPath(){
        path.Clear();
        hasPath = false;
    }

    private float ManhattanDistance(){
        float aux = 0;
        for (int i = 0; i < path.Count - 2; i++)
        {
            aux += Dist((Vector3)path[i].position , (Vector3)path[i + 1].position);
        }
        return aux;
    }

    private void WhiskersDetection(){
        if(Physics.Raycast(transform.position, transform.forward, 1f, obstacleLayer)){
            velocity -= transform.forward * Time.deltaTime * maxSpeed * 2;
        }
        
        if(Physics.Raycast(transform.position, (transform.forward + transform.right), 1f, obstacleLayer)){
            velocity -= transform.right * Time.deltaTime * maxSpeed * 2;
        }
        else if(Physics.Raycast(transform.position, (transform.forward - transform.right), 1f, obstacleLayer)){
            velocity += transform.right * Time.deltaTime * maxSpeed * 2;
        }
    }

    private float Dist(Vector3 a, Vector3 b){
        float difX = Mathf.Pow(b.x - a.x, 2);
        float difZ = Mathf.Pow(b.z - a.z, 2);
        return Mathf.Sqrt(difX + difZ);
    }
    private float GetRemainingDistance(){
        float difX = Mathf.Pow(path[actualNode].position.x - transform.position.x, 2);
        float difZ = Mathf.Pow(path[actualNode].position.z - transform.position.z, 2);
        remainingDistance = Mathf.Sqrt(difX + difZ);
        return remainingDistance;
    }

    private void OnDrawGizmos() {
        Debug.DrawRay(transform.position, transform.forward, Color.red);
        Debug.DrawRay(transform.position, (transform.forward + transform.right), Color.red);
        Debug.DrawRay(transform.position, (transform.forward - transform.right), Color.red);
        Debug.DrawRay(transform.position, velocity * 2, Color.green);
    }


}
