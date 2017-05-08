using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Sets up the grid using nodes
public class Grid : MonoBehaviour {

	public Vector2 gridWorldSize;
	public float nodeRadius;
	public LayerMask unwalkableMask;
	public bool displayGridGizmos = false;
	public TerrainType[] walkableRegions;
	LayerMask walkableMask;
	Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int, int>();

	Node[,] grid;

	float nodeDiameter;
	int gridSizeX, gridSizeY;

	// Performed when run button pressed
	void Awake() {
		
		nodeDiameter = nodeRadius * 2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

		InitTerrain ();
		CreateGrid ();

	}

	// Initialize terrain types
	void InitTerrain() {

		foreach (TerrainType region in walkableRegions) {
			walkableMask.value = walkableMask.value | region.terrainMask.value;
			walkableRegionsDictionary.Add ((int)Mathf.Log(region.terrainMask.value, 2),region.penalty);
		}

	}

	// Maximum grid size
	public int MaxSize {
		
		get {
			return gridSizeX * gridSizeY;
		}

	}

	// Create the grid used by A* search
	void CreateGrid() {
		
		grid = new Node[gridSizeX, gridSizeY];
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

		// Collision checks to check for walkability
		for (int x = 0; x < gridSizeX; x++) {
			for (int y = 0; y < gridSizeY; y++) {
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
				bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));

				int movementPenalty = 0;

				// If walkable, get penalties for each terrain type using raycasting
				if (walkable) {
					Ray ray = new Ray (worldPoint + Vector3.up*50, Vector3.down);
					RaycastHit hit;
					if (Physics.Raycast (ray, out hit, 100, walkableMask)) {
						walkableRegionsDictionary.TryGetValue (hit.collider.gameObject.layer, out movementPenalty);
					}
				}

				grid [x, y] = new Node (walkable, worldPoint, x, y, movementPenalty);
			}
		}

	}

	// Get neighbours of a node
	public List<Node> GetNeighbours (Node node) {

		List<Node> neighbours = new List<Node> ();

		// Loop
		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				// Skip if node is same as current node
				if (x == 0 && y == 0)
					continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				// Check if neighbour is inside the grid before adding to list
				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
					neighbours.Add (grid [checkX, checkY]);
				}
			}
		}

		return neighbours;

	}

	// Return a node when given (x, y, z) values
	public Node GetNodeFromWorldPoint(Vector3 worldPosition) {

		float percentX = (worldPosition.x / gridWorldSize.x) + .5f;
		float percentY = (worldPosition.z / gridWorldSize.y) + .5f;

		// Clamp values between 0 and 1
		percentX = Mathf.Clamp01 (percentX);
		percentY = Mathf.Clamp01 (percentY);

		int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

		return grid [x, y];

	}

	// Draw grid if specified
	void OnDrawGizmos() {
		
		Gizmos.DrawWireCube (transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

		if (grid != null && displayGridGizmos) {
			foreach (Node n in grid) {
				// Unwalkable nodes drawn in red, walkable areas drawn in white
				Gizmos.color = (n.walkable) ? Color.white : Color.red;
				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
			}
		}

	}

	// Object combining each terrain with an associated movement penalty
	[System.Serializable]
	public class TerrainType {

		public LayerMask terrainMask;
		public int penalty;

	}

}
