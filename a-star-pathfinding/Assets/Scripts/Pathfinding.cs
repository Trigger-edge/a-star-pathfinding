using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class Pathfinding : MonoBehaviour {

	public Transform seeker, target;
	Grid grid;

	void Awake() {
		
		grid = GetComponent<Grid> ();

	}

	void Update() {
		if (Input.GetButtonDown ("Jump")) {
			FindPath (seeker.position, target.position);
		}
	}

	void FindPath(Vector3 startPos, Vector3 targetPos) {

		Stopwatch sw = new Stopwatch ();
		sw.Start ();

		Node startNode = grid.GetNodeFromWorldPoint (startPos);
		Node targetNode = grid.GetNodeFromWorldPoint (targetPos);

		// open-set
		Heap<Node> openSet = new Heap<Node>(grid.MaxSize);

		// closed-set
		HashSet<Node> closedSet = new HashSet<Node>();

		openSet.Add (startNode);

		// Loop
		while (openSet.Count > 0) {
			Node currentNode = openSet.RemoveFirst ();
			closedSet.Add (currentNode);

			// If path found, retrace path
			if (currentNode == targetNode) {
				sw.Stop ();
				print ("Path found: " + sw.ElapsedMilliseconds + "ms.");
				RetracePath (startNode, targetNode);
				return;
			}

			foreach (Node neighbour in grid.GetNeighbours(currentNode)) {
				// If neighbour is unwalkable or already in closed-set, skip
				if (!neighbour.walkable || closedSet.Contains (neighbour)) {
					continue;
				}
					
				int newCostToNeighbour = currentNode.gCost + GetDistance (currentNode, neighbour);

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

	void RetracePath(Node startNode, Node targetNode) {

		// Retrace path backwards
		List<Node> path = new List<Node> ();
		Node currentNode = targetNode;

		while (currentNode != startNode) {
			path.Add (currentNode);
			currentNode = currentNode.parent;
		}

		// Reverse path
		path.Reverse ();

		// Test using Gizmos
		grid.path = path;

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
