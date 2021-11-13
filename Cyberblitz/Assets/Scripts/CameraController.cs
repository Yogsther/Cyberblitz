using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform cameraTransform;
    public Transform pivotTransform;

    public float cameraMovementSpeed = 10f;
    public float cameraRotationSpeed = 10f;

    public float cameraSmoothingSpeed = 10f;

    public Vector3 localCameraOffset;
    [Range(.55f, 1.5f)] public float zoom = 1f;
    public float minZoom = .55f;
    public float maxZoom = 1.4f;

    public float targetZoom = 1f;

    public float zoomSpeed = .1f;

    public float levelBorderPadding = 5f;
    public bool lookAtPivot;

    public LayerMask grabbableLayers;

    private bool isDragging;
    private Vector3 grabbedPoint;

    private Vector3 pivotSmoothPosition;
    private Vector3 pivotTargetPosition;
    private Vector3 cameraOffset => transform.TransformPoint(localCameraOffset / zoom);
    private float padding => levelBorderPadding / zoom;

    private Quaternion pivotTargetRotation;


    private Vector3 cameraMovementInput = Vector3.zero;
    private float cameraRotationInput = 0f;

    private Vector3 smoothCameraMovementInput = Vector3.zero;
    private float smoothCameraRotationInput = 0f;

    public void OnCameraMovementInput(InputAction.CallbackContext ctx)
    {
        cameraMovementInput = pivotTransform.TransformVector(ctx.ReadValue<Vector2>().ToFlatVector3());
    }

    public void OnCameraRotationInput(InputAction.CallbackContext ctx)
    {
        cameraRotationInput = ctx.ReadValue<float>();
    }

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
            pivotTargetRotation = pivotTransform.rotation;
        }
        else
        {
            Debug.Log("Did not find player");
        }

    }

    public void SmoothFocusOnPosition(Vector3 position)
    {

    }

    private void ChangeCameraZoom(float change)
    {
        targetZoom += change * zoomSpeed;

        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
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

    private void Update()
    {
        smoothCameraMovementInput = Vector3.Lerp(smoothCameraMovementInput, cameraMovementInput, cameraSmoothingSpeed * Time.deltaTime);
        smoothCameraRotationInput = Mathf.Lerp(smoothCameraRotationInput, cameraRotationInput, cameraSmoothingSpeed * Time.deltaTime);

        pivotTargetPosition += smoothCameraMovementInput * cameraMovementSpeed * Time.deltaTime;
        pivotTargetRotation *= Quaternion.AngleAxis(smoothCameraRotationInput * cameraRotationSpeed * Time.deltaTime, Vector3.up);








        
        zoom = Mathf.Lerp(zoom, targetZoom, 20f * Time.deltaTime);
    }

    private void LateUpdate()
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


        if (LevelManager.instance.currentLevel != null)
        {
            ClampPositionToFlatLevel(ref pivotTargetPosition);

            //Vector3 smoothToTarget = (pivotTargetPosition - pivotSmoothPosition);

            //pivotSmoothPosition += smoothToTarget * cameraSmoothingSpeed * Time.deltaTime;
        }


        pivotTransform.position = pivotTargetPosition;
        pivotTransform.rotation = pivotTargetRotation;
        cameraTransform.position = cameraOffset;

        if (lookAtPivot)
        {
            cameraTransform.LookAt(pivotTransform);
        }


    }


    private void OnDrawGizmos()
    {

        Gizmos.DrawLine(cameraOffset, pivotTransform.position);
    }

}
