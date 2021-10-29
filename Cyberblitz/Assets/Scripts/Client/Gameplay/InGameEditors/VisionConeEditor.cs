using System.Collections;
using UnityEngine;

public class VisionConeEditor : InGameEditor
{
	private VisionCone selectedVisionCone;

	private float inputDirection = 0f;

	[ContextMenu("Edit Test Cone")]
	public void EditTestPath()
	{
		VisionCone myVisionCone = new VisionCone();

		myVisionCone.origin = new GridPoint(new Vector2Int(5, 5));
		myVisionCone.radius = 5f;
		myVisionCone.angleWidth = 45f;

		EditVisionCone(ref myVisionCone);
	}

	public void EditVisionCone(ref VisionCone visionCone)
	{
		StartCoroutine(ConeEditing(visionCone));
	}

	public void StopEditing()
	{
		selectedVisionCone = null;
	}

	private IEnumerator ConeEditing(VisionCone visionCone)
	{
		selectedVisionCone = visionCone;

		yield return null;

		Debug.Log("[VisionConeEditor] - Started editing a cone");

		int groundLayer = 6;

		while (selectedVisionCone == visionCone)
		{

			if (!InputManager.isOnGui && InputManager.TryGetPointerHitLayer(groundLayer, out RaycastHit groundHit))
			{

				Vector2 mouseHitPoint = groundHit.point.FlatVector3ToVector2();

				Vector2 toMouse = mouseHitPoint - visionCone.origin.point;

				inputDirection = Mathf.Atan2(toMouse.y, toMouse.x) * Mathf.Rad2Deg;


				if (InputManager.pointerIsHeld)
				{
					visionCone.isSet = true;
					visionCone.direction = inputDirection;

					OnUpdated?.Invoke();

					GameManager.instance.TimelineEditor.DeselectBlock();
					selectedVisionCone = null;
				}

			}

			yield return null;
		}


		Debug.Log("[VisionConeEditor] - Stopped editing a cone");
	}


	private void OnDrawGizmos()
	{
		if (selectedVisionCone != null)
		{
			selectedVisionCone.DrawGizmo();

			Vector2[] unconfirmedConePoints = selectedVisionCone.GetConePoints(inputDirection);

			Gizmos.DrawLine(unconfirmedConePoints[0].ToFlatVector3(.01f), unconfirmedConePoints[1].ToFlatVector3(.01f));
			Gizmos.DrawLine(unconfirmedConePoints[0].ToFlatVector3(.01f), unconfirmedConePoints[2].ToFlatVector3(.01f));
		}
	}

}
