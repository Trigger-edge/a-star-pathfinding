using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// Requests a path
public class PathRequestManager : MonoBehaviour {

	Queue<PathRequest> pathRequestQueue = new Queue<PathRequest> ();
	PathRequest currentPathRequest;

	static PathRequestManager instance;
	Pathfinding pathfinding;

	bool isProcessingPath;

	// Performed when run button pressed
	void Awake() {

		instance = this;
		pathfinding = GetComponent<Pathfinding> ();

	}

	// Request path from one location to another
	public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback) {

		PathRequest newRequest = new PathRequest (pathStart, pathEnd, callback);
		instance.pathRequestQueue.Enqueue (newRequest);
		instance.TryProcessNext ();

	}

	// Calculate next path if no paths are being calculated
	void TryProcessNext () {

		if (!isProcessingPath && pathRequestQueue.Count > 0) {
			currentPathRequest = pathRequestQueue.Dequeue ();
			isProcessingPath = true;
			pathfinding.StartFindPath (currentPathRequest.pathStart, currentPathRequest.pathEnd);
		}

	}

	// Called when path has been calculated
	public void FinishedProcessingPath(Vector3[] path, bool success) {

		currentPathRequest.callback (path, success);
		isProcessingPath = false;
		TryProcessNext ();

	}

	// Struct to store start location, end location, and callback function
	struct PathRequest {

		public Vector3 pathStart;
		public Vector3 pathEnd;
		public Action<Vector3[], bool> callback;

		// Constructor
		public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback) {

			pathStart = _start;
			pathEnd = _end;
			callback = _callback;

		}

	}

}
