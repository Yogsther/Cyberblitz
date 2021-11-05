using UnityEngine;

public class CameraController : MonoBehaviour
{
	private Vector3 refpoint;
	private bool isDragging;

	public void InitCamera(Level level)
	{
		if(MatchManager.TryGetLocalPlayer(out Player localPlayer))
        {
			int team = localPlayer.team;

			SpawnArea spawnArea = level.spawnAreas[team];

			transform.position = spawnArea.center.ToFlatVector3();

			transform.Rotate(Vector3.up, spawnArea.cameraRotationForTeam, Space.World);// *= spawnArea.cameraRotation;    
		}
		   
	}

	private void LateUpdate()
	{
		if (LevelManager.instance.currentLevel != null)
		{

			if (InputManager.TryGetPointerHitLayer(6, out RaycastHit groundHit))
			{
				if (InputManager.rightButtonIsHeld)
				{
					if (!isDragging)
					{
						refpoint = groundHit.point;

						isDragging = true;
					}
					Vector3 diff = (refpoint - groundHit.point).Flatten();

					transform.position += diff;

				}
			}


			if (InputManager.rightButtonIsReleased)
			{
				isDragging = false;
			}

			Vector3 clampedPos = transform.position;

			clampedPos.x = Mathf.Clamp(clampedPos.x, 5f, LevelManager.instance.currentLevel.levelGridSize.x - 5f);
			clampedPos.z = Mathf.Clamp(clampedPos.z, 5f, LevelManager.instance.currentLevel.levelGridSize.y - 5f);

			transform.position = clampedPos;
		}
	}

}
