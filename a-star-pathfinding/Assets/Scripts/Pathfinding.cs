using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System;

// A* pathfinding class
public class Pathfinding : MonoBehaviour {

	PathRequestManager requestManager;
	Grid grid;

	public int heuristic;

	// Performed when run button pressed
	void Awake() {
		
		grid = GetComponent<Grid> ();
		requestManager = GetComponent<PathRequestManager> ();

		// SET HEURISTIC:
		// 0: Manhattan
		// 1: Euclidian
		// 2: Diagonals
		heuristic = 2;

	}

	// Start FindPath() as a coroutine
	public void StartFindPath(Vector3 startPos, Vector3 targetPos) {

		StartCoroutine(FindPath(startPos, targetPos));

	}

	// A* search
	IEnumerator FindPath(Vector3 startPos, Vector3 targetPos) {

		// Timer for testing
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

			// While open-set is not empty, do loop
			while (openSet.Count > 0) {
				Node currentNode = openSet.RemoveFirst ();
				closedSet.Add (currentNode);

				// If path found, retrace path
				if (currentNode == targetNode) {
					pathSuccess = true;
					// Stop timer and print time taken to find path
					sw.Stop ();
					print ("Path found: " + sw.ElapsedMilliseconds + "ms.");
					break;
				}

				foreach (Node neighbour in grid.GetNeighbours(currentNode)) {
					// If neighbour is unwalkable or already in closed-set, skip
					if (!neighbour.walkable || closedSet.Contains (neighbour)) {
						continue;
					}

					// Calculate heuristic costs
					int newCostToNeighbour;

					switch (heuristic) {
						case 0:
							newCostToNeighbour = currentNode.gCost + GetManhattanDistance (currentNode, neighbour) + neighbour.movementPenalty;
							break;
						case 1:
							newCostToNeighbour = currentNode.gCost + GetEuclideanDistance (currentNode, neighbour) + neighbour.movementPenalty;
							break;
						case 2:
							newCostToNeighbour = currentNode.gCost + GetDiagonalsDistance (currentNode, neighbour) + neighbour.movementPenalty;
							break;
						default:
							newCostToNeighbour = currentNode.gCost + GetEuclideanDistance (currentNode, neighbour) + neighbour.movementPenalty;
							break;
					}

					// If new path to neighbour is shorter or neighbour not in open-set
					if (newCostToNeighbour < neighbour.gCost || !openSet.Contains (neighbour)) {
						// Set neighbour's g-cost
						neighbour.gCost = newCostToNeighbour;

						// Set neighbour's h-cost
						switch (heuristic) {
							case 0:
								neighbour.hCost = GetManhattanDistance (neighbour, targetNode);
								break;
							case 1:
								neighbour.hCost = GetEuclideanDistance (neighbour, targetNode);
								break;
							case 2:
								neighbour.hCost = GetDiagonalsDistance (neighbour, targetNode);
								break;
							default:
								neighbour.hCost = GetEuclideanDistance (neighbour, targetNode);
								break;
						}

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

		// Retrace path if path found
		if (pathSuccess) {
			waypoints = RetracePath (startNode, targetNode);
		}

		requestManager.FinishedProcessingPath (waypoints, pathSuccess);

	}

	// Retrace path
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

	// Simplify path based on directions of current and next node 
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

	// Manhattan distance heuristic
	int GetManhattanDistance(Node nodeA, Node nodeB) {

		int distX = Mathf.Abs (nodeA.gridX - nodeB.gridX);
		int distY = Mathf.Abs (nodeA.gridY - nodeB.gridY);

		// Calculate Manhattan distance
		return distX + distY;

	}

	// Euclidian distance heuristic 
	int GetEuclideanDistance(Node nodeA, Node nodeB) {

		int distX = Mathf.Abs (nodeA.gridX - nodeB.gridX);
		int distY = Mathf.Abs (nodeA.gridY - nodeB.gridY);

		// Calculate Euclidean distance
		return (distX * distX) + (distY * distY);

	}

	// Diagonals distance heuristic 
	int GetDiagonalsDistance(Node nodeA, Node nodeB) {

		int distX = Mathf.Abs (nodeA.gridX - nodeB.gridX);
		int distY = Mathf.Abs (nodeA.gridY - nodeB.gridY);

		if (distX > distY) {
			// Calculate number of diagonal nodes to get to the same y value as nodeB, then calculate number
			// of horizontal nodes to get to nodeB
			return 14 * distY + 10 * (distX - distY);
		}

		 // Calculate number of diagonal nodes to get to the same x value as nodeB, then calculate number
		 // of vertical nodes to get to nodeB
		return 14 * distX + 10 * (distY - distX);

	}

}
