using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;

public class Pathfinding : MonoBehaviour {

	PathRequestManager requestManager;
	Grid grid;

	void Awake() {
		
		grid = GetComponent<Grid> ();
		requestManager = GetComponent<PathRequestManager> ();

	}

	public void StartFindPath(Vector3 startPos, Vector3 targetPos) {

		StartCoroutine(FindPath(startPos, targetPos));

	}

	IEnumerator FindPath(Vector3 startPos, Vector3 targetPos) {

		Stopwatch sw = new Stopwatch ();
		sw.Start ();

		Vector3[] waypoints = new Vector3[0];
		bool pathSuccess = false;

		Node startNode = grid.GetNodeFromWorldPoint (startPos);
		Node targetNode = grid.GetNodeFromWorldPoint (targetPos);

		if (startNode.walkable && targetNode.walkable) {
			// open-set
			Heap<Node> openSet = new Heap<Node> (grid.MaxSize);

			// closed-set
			HashSet<Node> closedSet = new HashSet<Node> ();

			openSet.Add (startNode);

			// Loop
			while (openSet.Count > 0) {
				Node currentNode = openSet.RemoveFirst ();
				closedSet.Add (currentNode);

				// If path found, retrace path
				if (currentNode == targetNode) {
					pathSuccess = true;
					sw.Stop ();
					print ("Path found: " + sw.ElapsedMilliseconds + "ms.");
					break;
				}

				foreach (Node neighbour in grid.GetNeighbours(currentNode)) {
					// If neighbour is unwalkable or already in closed-set, skip
					if (!neighbour.walkable || closedSet.Contains (neighbour)) {
						continue;
					}
						
					int newCostToNeighbour = currentNode.gCost + GetDistance (currentNode, neighbour) + neighbour.movementPenalty;

					// If new path to neighbour is shorter or neighbour not in open-set
					if (newCostToNeighbour < neighbour.gCost || !openSet.Contains (neighbour)) {
						// Set neighbour's new f-cost
						neighbour.gCost = newCostToNeighbour;
						neighbour.hCost = GetDistance (neighbour, targetNode);

						// Set parent of neighbour to current node
						neighbour.parent = currentNode;

						// If neighbour not in open-set, add it to open-set
						if (!openSet.Contains (neighbour)) {
							openSet.Add (neighbour);
							openSet.UpdateItem (neighbour);
						}
					}
				}
			}
		}

		yield return null;

		if (pathSuccess) {
			waypoints = RetracePath (startNode, targetNode);
		}

		requestManager.FinishedProcessingPath (waypoints, pathSuccess);

	}

	Vector3[] RetracePath(Node startNode, Node targetNode) {

		// Retrace path backwards
		List<Node> path = new List<Node> ();
		Node currentNode = targetNode;

		while (currentNode != startNode) {
			path.Add (currentNode);
			currentNode = currentNode.parent;
		}

		Vector3[] waypoints = SimplifyPath (path);

		// Reverse path
		Array.Reverse(waypoints);

		return waypoints;

	}

	Vector3[] SimplifyPath(List<Node> path) {

		List<Vector3> waypoints = new List<Vector3> ();
		Vector2 directionOld = Vector2.zero;

		for (int i = 1; i < path.Count; i++) {
			Vector2 directionNew = new Vector2 (path [i - 1].gridX - path [i].gridX, path [i - 1].gridY - path [i].gridY);

			if (directionNew != directionOld) {
				waypoints.Add (path [i].worldPosition);
			}
			directionOld = directionNew;
		}

		return waypoints.ToArray ();

	}

	int GetDistance(Node nodeA, Node nodeB) {

		// Heuristic
		int distX = Mathf.Abs (nodeA.gridX - nodeB.gridX);
		int distY = Mathf.Abs (nodeA.gridY - nodeB.gridY);

		if (distX > distY)
			return 14 * distY + 10 * (distX - distY);
		
		return 14 * distX + 10 * (distY - distX);

	}

}
