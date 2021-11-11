using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform cameraTransform;
    public Transform pivotTransform;


    public Vector3 localCameraOffset;
    [Range(.55f, 1.5f)] public float zoom = 1f;
    public float minZoom = .55f;
    public float maxZoom = 1.4f;

    public float zoomSpeed = .1f;

    public float levelBorderPadding = 5f;
    public bool lookAtPivot;

    public LayerMask grabbableLayers;

    private bool isDragging;
    private Vector3 grabbedPoint;


    private Vector3 pivotTargetPosition;
    private Vector3 cameraOffset => transform.TransformPoint(localCameraOffset / zoom);
    private float padding => levelBorderPadding / zoom;


    private void Awake()
    {
        InputManager.OnCameraZoom += ChangeCameraZoom;
    }

    public void InitCamera(Level level)
    {
        pivotTransform.position = Vector3.zero;
        pivotTransform.rotation = Quaternion.identity;
        pivotTransform.localScale = Vector3.one;

        if (MatchManager.TryGetLocalPlayer(out Player localPlayer))
        {
            int team = localPlayer.team;

            SpawnArea spawnArea = level.spawnAreas[team];

            pivotTransform.position = spawnArea.center.ToFlatVector3();

            pivotTransform.Rotate(Vector3.up, spawnArea.cameraRotationForTeam, Space.World);// *= spawnArea.cameraRotation;    

            pivotTargetPosition = pivotTransform.position;
        }
        else
        {
            Debug.Log("Did not find player");
        }

    }

    private void ChangeCameraZoom(float change)
    {
        zoom += change * zoomSpeed * Time.deltaTime;

        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
    }

    private void ClampPositionToFlatLevel(ref Vector3 position)
    {
        Vector2 levelSize = LevelManager.instance.currentLevel.levelGridSize;


        Vector2 clampedPadding = new Vector2
        {
            x = Mathf.Clamp(padding, 0f, levelSize.x * .5f),
            y = Mathf.Clamp(padding, 0f, levelSize.y * .5f)
        };

        position.x = Mathf.Clamp(position.x, clampedPadding.x, levelSize.x - clampedPadding.x);
        position.z = Mathf.Clamp(position.z, clampedPadding.y, levelSize.y - clampedPadding.y);
    }

    private void LateUpdate()
    {
        if (LevelManager.instance.currentLevel != null)
        {

            if (InputManager.TryGetPointerHitLayer(grabbableLayers, out RaycastHit groundHit) && InputManager.rightButtonIsHeld)
            {

                if (!isDragging)
                {
                    grabbedPoint = groundHit.point;

                    isDragging = true;
                }

                Vector3 toGrabbedPoint = (grabbedPoint - groundHit.point).Flatten();

                pivotTargetPosition += toGrabbedPoint;
            }


            if (InputManager.rightButtonIsReleased)
            {
                isDragging = false;
            }



            ClampPositionToFlatLevel(ref pivotTargetPosition);
            

            pivotTransform.position = pivotTargetPosition;
            cameraTransform.position = cameraOffset;

            if (lookAtPivot)
            {
                cameraTransform.LookAt(pivotTransform);
            }
        }
    }


    private void OnDrawGizmos()
    {

        Gizmos.DrawLine(cameraOffset, pivotTransform.position);
    }

}
