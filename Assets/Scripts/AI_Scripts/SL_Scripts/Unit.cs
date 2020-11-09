using UnityEngine;
using System.Collections;
using System;

public class Unit : MonoBehaviour {

	//
	//Public Variables
	//
	public Transform target;
	public float speed = 5;
	public float turnSpeed = 3;
	public float turnDst = 1;
	public float stoppingDst = 1;
	public LayerMask layerMask;

	//
	//Private Variables
	//
	private bool followingPath = false;
	private bool pathfound = false;
	private int cont;
	private bool finishPath = false;
	private bool returning;
	private const float minPathUpdateTime = .2f;
	private const float pathUpdateMoveThreshold = .5f;
	private Path path;

    void Start() {
		followingPath = false;
		finishPath = false;
		pathfound = false;
		returning = false;
		cont = 0;
	}

	/// <summary>
	/// Update is called every frame, if the MonoBehaviour is enabled.
	/// </summary>
	void Update()
	{
		SetDynamicDestination(target);
	}

    public void SetDynamicDestination(Transform destination)
	{
		if (!followingPath && !pathfound)
		{
			pathfound = true;
			StartCoroutine(DynamicMovemnet(destination));
		}
	}

	IEnumerator DynamicMovemnet(Transform destination)
    {
		yield return new WaitForSeconds(1f);
		
		StopCoroutine("UpdatePath");
		target = destination;
		StartCoroutine(UpdatePath(0.01f));
	}

	public void SetDestination(Transform destination)
	{
		if (!followingPath && !pathfound)
		{
			pathfound = true;
			StopCoroutine("UpdatePath");
			target = destination;
			StartCoroutine(UpdatePath(.5f));
		}
	}

	public void SetPatrol(Transform[] wayPoints)
    {
		if (!followingPath && !pathfound)
		{
			pathfound = true;
			StopCoroutine("UpdatePath");
			target = wayPoints[cont];
			StartCoroutine(UpdatePath(1f));
			if (!returning && cont < wayPoints.Length) cont++;
			else if (returning && cont > 0) cont--;
			if (cont == 0 || cont == wayPoints.Length - 1) returning = !returning;
		}
	}

    public void OnPathFound(Vector3[] waypoints, bool pathSuccessful) {
		if (pathSuccessful) {
			path = new Path(waypoints, transform.position, turnDst, stoppingDst);
			pathfound = true;
			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
		}
	}

	IEnumerator UpdatePath(float secondsToWait) {
		if (finishPath) {
			yield return new WaitForSeconds (secondsToWait);
			finishPath = false;
		}
		PathRequestManager.RequestPath (new PathRequest(transform.position, target.position, OnPathFound));

		float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
		Vector3 targetPosOld = target.position;

		while (true) {
			yield return new WaitForSeconds (minPathUpdateTime);
			print (((target.position - targetPosOld).sqrMagnitude) + "    " + sqrMoveThreshold);
			if ((target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold) {
				PathRequestManager.RequestPath (new PathRequest(transform.position, target.position, OnPathFound));
				targetPosOld = target.position;
			}
		}
	}

	private Vector3 playerDistanceToactualNode;
    private Vector3 desiredVelocity; 
    private Vector3 steering;
	private Vector3 velocity;

	IEnumerator FollowPath() {
		followingPath = true;
		int pathIndex = 0;
		transform.LookAt (path.lookPoints [0]);

		float speedPercent = 1;

		while (followingPath) {
			Vector2 pos2D = new Vector2 (transform.position.x, transform.position.z);
			while (path.turnBoundaries [pathIndex].HasCrossedLine (pos2D)) {
				if (pathIndex == path.finishLineIndex) {
					followingPath = false;
					finishPath = true;
					pathfound = false;
					break;
				} else {
					pathIndex++;
				}
			}

			if (followingPath) {

				if (pathIndex >= path.slowDownIndex && stoppingDst > 0) {
					speedPercent = Mathf.Clamp01 (path.turnBoundaries [path.finishLineIndex].DistanceFromPoint (pos2D) / stoppingDst);
					if (speedPercent < 0.01f) {
						finishPath = true;
						followingPath = false;
						pathfound = false;
					}
				}

				/*Quaternion targetRotation = Quaternion.LookRotation (path.lookPoints [pathIndex] - transform.position);
				transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
				//calculateWiskers(speedPercent);
				transform.Translate (Vector3.forward * Time.deltaTime * speed * speedPercent, Space.Self);*/

				playerDistanceToactualNode = path.lookPoints[pathIndex] - transform.position;
				desiredVelocity = playerDistanceToactualNode.normalized * speed;
				steering = desiredVelocity * 0.8f - velocity;

				velocity += steering * Time.deltaTime * speedPercent;

				Quaternion rotation = Quaternion.LookRotation(velocity, Vector3.up);
				transform.rotation = rotation;

				WhiskersDetection();
				transform.position += velocity * Time.deltaTime;
			}

			yield return null;

		}
	}

    private void WhiskersDetection(){
        if(Physics.Raycast(transform.position, transform.forward, 1.4f, layerMask)){
            velocity -= transform.forward * Time.deltaTime * speed;
        }
        
        if(Physics.Raycast(transform.position, (transform.forward + transform.right), 1f, layerMask)){
            velocity -= transform.right * Time.deltaTime * speed * 2f;
        }
        if(Physics.Raycast(transform.position, (transform.forward - transform.right), 1f, layerMask)){
            velocity += transform.right * Time.deltaTime * speed * 2f;
        }
    }

    public void OnDrawGizmos() {
		if (path != null) {
			path.DrawWithGizmos ();
		}

		Debug.DrawRay(transform.position, transform.forward * 1.5f,Color.red);
		Debug.DrawRay(transform.position, (transform.forward + transform.right),Color.red);
		Debug.DrawRay(transform.position, (transform.forward - transform.right),Color.red);
	}
}
