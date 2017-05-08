using UnityEngine;
using System.Collections;

// Node class
public class Node : IHeapItem<Node> {

	public bool walkable;
	public Vector3 worldPosition;
	public int gridX;
	public int gridY;
	public int movementPenalty;

	public int gCost;
	public int hCost;
	public Node parent;
	int heapIndex;

	// Constructor
	public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, int _penalty) {
		
		walkable = _walkable;
		worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
		movementPenalty = _penalty;

	}

	// F-cost (= g-cost + h-cost)
	public int fCost {
		
		get {
			return gCost + hCost;
		}

	}

	// Heap index
	public int HeapIndex {
		
		get {
			return heapIndex;
		}
		set {
			heapIndex = value;
		}

	}

	// Compare nodes (using IComparable<Node>)
	public int CompareTo(Node nodeToCompare) {

		int compare = fCost.CompareTo (nodeToCompare.fCost);

		// Compare h-costs in case f-costs are the same
		if (compare == 0) {
			compare = hCost.CompareTo (nodeToCompare.hCost);
		}

		return -compare;

	}

}
