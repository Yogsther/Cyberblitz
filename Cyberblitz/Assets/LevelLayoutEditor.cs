using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelLayoutEditor : MonoBehaviour
{
    public LevelLayout layout;

	private List<LevelElement> GetLevelElements()
    {
		List<LevelElement> levelElements = new List<LevelElement>();
		if(layout != null)
        {
			levelElements.AddRange(layout.spawnAreas);
			levelElements.AddRange(layout.gridColliders);
        }
		return levelElements;
    }

	private void OnDrawGizmos()
	{
		if (layout != null)
		{

#if UNITY_EDITOR
			Handles.Label(Vector3.right * layout.levelGridSize.x * .5f, $"W: {layout.levelGridSize.x}");
			Handles.Label(Vector3.forward * layout.levelGridSize.y * .5f, $"H: {layout.levelGridSize.y}");
#endif
			DrawGizmosGrid();



			foreach (LevelElement element in GetLevelElements()) element.DrawGizmo();
		}
	}

	private void DrawGizmosGrid()
	{

		for (float y = -.5f; y < layout.levelGridSize.y + .5f; y++)
		{
			float x1 = -.5f;
			float x2 = layout.levelGridSize.x - .5f;

			Vector3 fromPoint = new Vector2(x1, y).ToFlatVector3();
			Vector3 toPoint = new Vector2(x2, y).ToFlatVector3();

			Gizmos.DrawLine(fromPoint, toPoint);
		}

		for (float x = -.5f; x < layout.levelGridSize.x + .5f; x++)
		{
			float y1 = -.5f;
			float y2 = layout.levelGridSize.y - .5f;

			Vector3 fromPoint = new Vector2(x, y1).ToFlatVector3();
			Vector3 toPoint = new Vector2(x, y2).ToFlatVector3();

			Gizmos.DrawLine(fromPoint, toPoint);
		}
	}
}
