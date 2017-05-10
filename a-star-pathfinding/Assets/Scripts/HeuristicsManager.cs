using UnityEngine;
using System.Collections;

// Heuristics
public class HeuristicsManager : MonoBehaviour {

	// Manhattan distance heuristic
	public int GetManhattanDistance(Node nodeA, Node nodeB) {

		int distX = Mathf.Abs (nodeA.gridX - nodeB.gridX);
		int distY = Mathf.Abs (nodeA.gridY - nodeB.gridY);

		// Calculate Manhattan distance
		return distX + distY;

	}

	// Euclidian distance heuristic 
	public int GetEuclideanDistance(Node nodeA, Node nodeB) {

		int distX = Mathf.Abs (nodeA.gridX - nodeB.gridX);
		int distY = Mathf.Abs (nodeA.gridY - nodeB.gridY);

		// Calculate Euclidean distance
		return (distX * distX) + (distY * distY);

	}

	// Diagonals distance heuristic 
	public int GetDiagonalsDistance(Node nodeA, Node nodeB) {

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
