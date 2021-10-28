using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AutoGridPathEditor : InGameEditor
{
	private AutoGridPath selectedGridPath;

	private Vector3 smoothPos;

	private readonly float smoothRot;

    /*[ContextMenu("Edit Test Path")]
    public void EditTestPath()
    {
        AutoGridPath myGridPath = new AutoGridPath(Vector2Int.zero);

        EditGridPath(ref myGridPath);
    }*/

	public void EditGridPath(ref AutoGridPath gridPath, float maxLength = -1f)
	{
		StartCoroutine(PathEditing(gridPath, maxLength));
	}

	[ContextMenu("Stop Editing")]
	public void StopEditing()
	{
		selectedGridPath = null;
	}

	private IEnumerator PathEditing(AutoGridPath gridPath, float maxLength = -1f)
	{
		selectedGridPath = gridPath;

		yield return null;

		Debug.Log("[GridPathEditor] - Started editing a path");


		int groundLayer = 6;

		while (selectedGridPath == gridPath)
		{


			if (!InputManager.isOnGui && InputManager.TryGetPointerHitLayer(groundLayer, out RaycastHit groundHit) && InputManager.pointerIsHeld)
			{


				Vector2 mousePoint = groundHit.point.FlatVector3ToVector2();
				Vector2Int point = mousePoint.RoundToVector2Int();

				bool isBlocked = Physics.CheckSphere(point.ToFlatVector3(.5f), .25f);

				if (!isBlocked)
				{
					point = (gridPath.origin.point + Vector2.ClampMagnitude(point - gridPath.origin.point, maxLength)).RoundToVector2Int();

					gridPath.target.point = point;
					OnUpdated?.Invoke();

				}
			}

			yield return null;
		}

		Debug.Log($"[GridPathEditor] - Stopped editing a path");
	}

	private void OnDrawGizmos()
	{
		if (selectedGridPath != null)
		{
			selectedGridPath.DrawGizmos();
		}
	}
}
