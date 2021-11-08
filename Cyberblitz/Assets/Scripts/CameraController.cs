using UnityEngine;

public class CameraController : MonoBehaviour
{
	private Vector3 refpoint;
	private bool isDragging;

	private Vector3 targetPosition;

	private Vector3 smoothVelocity;


	public void InitCamera(Level level)
	{
		if (MatchManager.TryGetLocalPlayer(out Player localPlayer))
		{
			int team = localPlayer.team;

			SpawnArea spawnArea = level.spawnAreas[team];

			transform.position = spawnArea.center.ToFlatVector3();

			transform.Rotate(Vector3.up, spawnArea.cameraRotationForTeam, Space.World);// *= spawnArea.cameraRotation;    

			targetPosition = transform.position;
		} else
		{
			Debug.Log("Did not find player");
		}

	}

	private void LateUpdate()
	{
		if (LevelManager.instance.currentLevel != null)
		{

			if (InputManager.TryGetPointerHitLayer(LayerMask.GetMask("Ground"), out RaycastHit groundHit))
			{
				if (InputManager.rightButtonIsHeld)
				{
					if (!isDragging)
					{
						refpoint = groundHit.point;

						isDragging = true;
					}
					Vector3 diff = (refpoint - groundHit.point).Flatten();

					targetPosition += diff;

				}
			}


			if (InputManager.rightButtonIsReleased)
			{
				isDragging = false;
			}

			Vector3 clampedPos = targetPosition;

			clampedPos.x = Mathf.Clamp(clampedPos.x, 5f, LevelManager.instance.currentLevel.levelGridSize.x - 5f);
			clampedPos.z = Mathf.Clamp(clampedPos.z, 5f, LevelManager.instance.currentLevel.levelGridSize.y - 5f);

			targetPosition = clampedPos;



			transform.position = targetPosition; //Vector3.MoveTowards(transform.position, targetPosition, .1f); //Vector3.SmoothDamp(transform.position, targetPosition, ref smoothVelocity, .2f);
		}
	}

}
