using UnityEngine;
using System.Collections;
using System;

public class Unit : MonoBehaviour {

	//
	//Public Variables
	//
	public float speed = 5;
	public float turnSpeed = 3;
	public float turnDst = 1;
	public float stoppingDst = 1;
	public LayerMask layerMask;
	public bool finishPath = false;

	//
	//Private Variables
	//
	private bool followingPath = false;
	private bool pathfound = false;
	private int cont;
	private bool returning;
	private const float minPathUpdateTime = .2f;
	private const float pathUpdateMoveThreshold = .5f;
	private Path path;
    private bool onlyOneDestination;

    void Start() {
		followingPath = false;
		finishPath = false;
		pathfound = false;
		returning = false;
		cont = 0;
	}

    /// <summary>
    /// Llamar en un Update, le pasas el Transform del palyer que se esta moviendo dinamicamente.
    /// </summary>
    /// <param name="destination"></param>
    public void SetDynamicDestination(Transform destination)
	{
		if (!followingPath && !pathfound)
		{
            onlyOneDestination = false;
            pathfound = true;
			StartCoroutine(DynamicMovemnet(destination));
		}
	}

	IEnumerator DynamicMovemnet(Transform destination)
    {
		yield return new WaitForSeconds(1f);
		
		StopCoroutine("UpdatePath");
		StartCoroutine(UpdatePath(0.01f, destination.position));
	}
    /// <summary>
    /// Llamar una sola vez, le pasas un punto y va a el, al llegar, finishPath se pone a true
    /// </summary>
    /// <param name="destination"></param>
	public void SetDestination(Vector3 destination)
	{
		if (!followingPath && !pathfound)
		{
            onlyOneDestination = true;
            pathfound = true;
			StopCoroutine("UpdatePath");
			StartCoroutine(UpdatePath(.5f, destination));
		}
	}
    /// <summary>
    /// Poner en el UPDATE, pasarle un array de Transform[] con los wayPoints, parara el tiempo dado por ti en cada wayPoint
    /// </summary>
    /// <param name="wayPoints"></param>
    /// <param name="timeStoppedInWaypoint"></param>
	public void SetPatrol(Transform[] wayPoints, float timeStoppedInWaypoint) 
    {
		if (!followingPath && !pathfound)
		{
            onlyOneDestination = false;
            pathfound = true;
			StopCoroutine("UpdatePath");
			StartCoroutine(UpdatePath(timeStoppedInWaypoint, wayPoints[cont].position));
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

	IEnumerator UpdatePath(float secondsToWait, Vector3 target) {
        if (finishPath) {
			yield return new WaitForSeconds (secondsToWait);
			finishPath = false;
		}
		PathRequestManager.RequestPath (new PathRequest(transform.position, target, OnPathFound));

		float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
		Vector3 targetPosOld = target;

		while (true) {
			yield return new WaitForSeconds (minPathUpdateTime);
			//print (((target.position - targetPosOld).sqrMagnitude) + "    " + sqrMoveThreshold);
			if (!onlyOneDestination && (target - targetPosOld).sqrMagnitude > sqrMoveThreshold) {
				PathRequestManager.RequestPath (new PathRequest(transform.position, target, OnPathFound));
				targetPosOld = target;
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

				Quaternion targetRotation = Quaternion.LookRotation (path.lookPoints [pathIndex] - transform.position);
				transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
				//calculateWiskers(speedPercent);
				transform.Translate (Vector3.forward * Time.deltaTime * speed * speedPercent, Space.Self);

				/*playerDistanceToactualNode = path.lookPoints[pathIndex] - transform.position;
				desiredVelocity = playerDistanceToactualNode.normalized * speed;
				steering = desiredVelocity * 0.8f - velocity;

				velocity += steering * Time.deltaTime * speedPercent;

				Quaternion rotation = Quaternion.LookRotation(velocity, Vector3.up);
				transform.rotation = rotation;

				WhiskersDetection();
				transform.position += velocity * Time.deltaTime;*/
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
