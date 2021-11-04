using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace WayStarPathfinding
{
	public class PathFinder
	{
		/// <summary>The starting position of the path</summary>
		public Vector2Int origin;
		/// <summary>The target position of the path</summary>
		public Vector2Int target;

		public static Vector2Int gridSize = new Vector2Int(1000, 1000);

		public LayerMask blockingLayerMask;

		public List<Node> openNodes = new List<Node>();
		public List<Node> closedNodes = new List<Node>();

		public List<Node> pathNodes = new List<Node>();

		public List<Vector2Int> points = new List<Vector2Int>();

		public LayerMask blockMask;


		private static Vector2Int[] GetNeighborCoords(Vector2Int position)
		{
			return new Vector2Int[8]
			{
			position + Vector2Int.up,
			position + Vector2Int.down,
			position + Vector2Int.left,
			position + Vector2Int.right,
			position + Vector2Int.up + Vector2Int.left,
			position + Vector2Int.up + Vector2Int.right,
			position + Vector2Int.down + Vector2Int.left,
			position + Vector2Int.down + Vector2Int.right
			};
		}

		public List<Vector2Int> GetPath(Vector2Int origin, Vector2Int target, int maxLoops = 1000)
		{
			target.x = Mathf.Clamp(target.x, 0, gridSize.x - 1);
			target.y = Mathf.Clamp(target.y, 0, gridSize.y - 1);

			bool targetIsBlocked = Physics.CheckSphere(target.ToFlatVector3(.5f), .25f, blockMask);

            if (targetIsBlocked)
            {
				Debug.LogWarning("Target was blocked");
				return points;
            }

			if (origin == this.origin && target == this.target)
			{
				return points;
			}



			

			this.origin = origin;
			this.target = target;

			

			openNodes.Clear();
			closedNodes.Clear();
			pathNodes.Clear();
			points.Clear();


			Node originNode = new Node(origin, target);
			Node targetNode = new Node(target, target);

			openNodes.Add(originNode);



			for (int i = 0; i < maxLoops; i++)
			{
				Node current = GetOpenNodeWithLowestFCost();

				openNodes.Remove(current);
				closedNodes.Add(current);

				if (current.position == target)
				{
					targetNode = current;
					break;
				}



				foreach (Node neighbor in GetNodeNeighbors(current))
				{

					bool isBlocked = Physics.CheckSphere(neighbor.position.ToFlatVector3(.5f), .25f, blockMask);

					if (!(isBlocked || TryGetNodeAtPosition(closedNodes, neighbor.position, out Node nodeAtPos)))
					{



						bool neighborIsInOpen = openNodes.Contains(neighbor);


						float newGCost = current.gCost + Vector2Int.Distance(current.position, neighbor.position);

						bool newPathIsShorter = newGCost < neighbor.gCost;

						if (newPathIsShorter || !neighborIsInOpen)
						{
							neighbor.parent = current;

							if (!neighborIsInOpen)
							{
								openNodes.Add(neighbor);
							}
						}
					}
				}

			}


			for (Node node = targetNode; node != null; node = node.parent)
			{
				closedNodes.Remove(node);
				pathNodes.Add(node);

				points.Add(node.position);
			}

			points.Reverse();

			return points;
		}


		private Node GetOpenNodeWithLowestFCost()
		{
			Node result = null;
			float lowestFCost = 2000f;

			foreach (Node node in openNodes)
			{
				if (node.fCost < lowestFCost)
				{
					result = node;
					lowestFCost = node.fCost;
				}
			}

			return result;
		}

		private bool TryGetNodeAtPosition(List<Node> nodeList, Vector2Int position, out Node output)
		{
			output = null;

			foreach (Node node in nodeList)
			{
				if (node.position == position)
				{
					output = node;
					break;
				}
			}

			return output != null;
		}

		private List<Node> GetNodeNeighbors(Node node)
		{
			List<Node> neighbors = new List<Node>();

			foreach (Vector2Int neighboringCoord in GetNeighborCoords(node.position))
			{
				if (TryGetNodeAtPosition(openNodes, neighboringCoord, out Node neighboringNode))
				{
					neighbors.Add(neighboringNode);
				} else
				{
					Node newNode = new Node(neighboringCoord, target, node);
					neighbors.Add(newNode);
				}
			}

			return neighbors;
		}

		public void DrawGizmos()
		{

			Gizmos.color = Color.green;
			foreach (Node node in openNodes)
			{
				node.DrawGizmo();
			}

			Gizmos.color = Color.red;
			foreach (Node node in closedNodes)
			{
				node.DrawGizmo();
			}

			Gizmos.color = Color.blue;
			foreach (Node node in pathNodes)
			{
				node.DrawGizmo();
			}
		}



	}
}

