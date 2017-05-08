using UnityEngine;
using System.Collections;

// Manages agent object
public class Unit : MonoBehaviour {

	public Transform target;
	float speed = 20;
	Vector3[] path;
	int targetIndex;

	// Performed when run button pressed
	void Start() {

		PathRequestManager.RequestPath (transform.position, target.position, OnPathFound);

	}

	// Start FollowPath coroutine if path found
	public void OnPathFound(Vector3[] newPath, bool pathSuccessful) {

		if (pathSuccessful) {
			path = newPath;
			StopCoroutine ("FollowPath");
			StartCoroutine ("FollowPath");
		}

	}

	// Coroutine to move agent along path
	IEnumerator FollowPath() {

		Vector3 currentWaypoint = path [0];

		while (true) {
			if (transform.position == currentWaypoint) {
				targetIndex++;
				if (targetIndex >= path.Length) {
					yield break;
				}
				currentWaypoint = path [targetIndex];
			}

			transform.position = Vector3.MoveTowards (transform.position, currentWaypoint, speed * Time.deltaTime);
			yield return null;
		}

	}

	// Draw path
	public void OnDrawGizmos() {

		if (path != null) {
			for (int i = targetIndex; i < path.Length; i++) {
				// Path drawn in black
				Gizmos.color = Color.black;
				Gizmos.DrawCube (path [i], Vector3.one);

				if (i == targetIndex) {
					Gizmos.DrawLine (transform.position, path [i]);
				} else {
					Gizmos.DrawLine (path [i - 1], path [i]);
				}
			}
		}

	}

}
