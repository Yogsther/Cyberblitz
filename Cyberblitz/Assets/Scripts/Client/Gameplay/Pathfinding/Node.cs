

using UnityEditor;
using UnityEngine;

namespace WayStarPathfinding
{
    public class Node
    {
        public Vector2Int position;
        public Vector2Int target;

        public Node parent;

        public float gCost => parent != null ? parent.gCost + Vector2Int.Distance(parent.position, position) : 0f;
        public float hCost => Vector2Int.Distance(position, target);
        public float fCost => gCost + hCost;



        public Node(Vector2Int position, Vector2Int target)
        {
            this.position = position;
            this.target = target;
            parent = null;
        }

        public Node(Vector2Int position, Vector2Int target, Node parent)
        {
            this.position = position;
            this.target = target;
            this.parent = parent;
        }

        public void DrawGizmo()
        {
            Vector3 worldPos = position.ToFlatVector3(.5f);

#if UNITY_EDITOR
            Handles.Label(worldPos + Vector3.left * .4f + Vector3.forward * .4f, $"g:{Mathf.Round(gCost * 10f)}");
            Handles.Label(worldPos + Vector3.left * .4f + Vector3.forward * .2f, $"h:{Mathf.Round(hCost * 10f)}");
            Handles.Label(worldPos + Vector3.left * .4f, $"f:{Mathf.Round(fCost * 10f)}");
#endif

            Gizmos.DrawWireCube(worldPos, Vector3.one);
        }
    }
}