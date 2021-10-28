using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static Vector2 pointerScreenPosition;
    public static Vector2 pointerScreenPositionDelta;
    public static RaycastHit pointerHitInfo;
    public static RaycastHit[] pointerHitInfos;

    public static bool pointerHit;
    public static bool isOnGui;

    public static bool pointerIsPressed;
    public static bool pointerIsHeld;
    public static bool pointerIsReleased;

    public static bool rightButtonIsPressed;
    public static bool rightButtonIsHeld;
    public static bool rightButtonIsReleased;

    public LayerMask pointerLayers;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }


    public void OnPointerMove(InputAction.CallbackContext ctx)
    {
        pointerScreenPosition = ctx.ReadValue<Vector2>();
    }

    public void OnPointerMoveDelta(InputAction.CallbackContext ctx)
    {
        pointerScreenPositionDelta = ctx.ReadValue<Vector2>();
    }

    public void OnPointerClick(InputAction.CallbackContext ctx)
    {
        pointerIsPressed = ctx.started;
        pointerIsHeld = ctx.performed;
        pointerIsReleased = ctx.canceled;
    }

    public void OnRightButtonClick(InputAction.CallbackContext ctx)
    {
        rightButtonIsPressed = ctx.started;
        rightButtonIsHeld = ctx.performed;
        rightButtonIsReleased = ctx.canceled;
    }


    private void Update()
    {
        isOnGui = EventSystem.current.IsPointerOverGameObject();

        PerformPointerRaycast();
    }

    private void PerformPointerRaycast()
    {
        Ray pointerRay = mainCamera.ScreenPointToRay(pointerScreenPosition);

        pointerHit = Physics.Raycast(pointerRay, out pointerHitInfo, 100f, pointerLayers);
        pointerHitInfos = Physics.RaycastAll(pointerRay, 100f, pointerLayers);
    }

    public static bool TryGetPointerHitLayer(LayerMask layerMask, out RaycastHit raycastHit)
    {
        if (pointerHitInfos != null)
        {

            foreach (RaycastHit hit in pointerHitInfos)
            {

                bool hitObjectIsInLayerMask = layerMask == hit.collider.gameObject.layer;

                if (hitObjectIsInLayerMask)
                {
                    raycastHit = hit;

                    return true;
                }
            }
        }

        raycastHit = new RaycastHit();

        return false;
    }


    private void OnDrawGizmosSelected()
    {

        if (Application.isPlaying)
        {
            foreach (RaycastHit hit in pointerHitInfos)
            {
                Gizmos.DrawWireSphere(hit.point, .1f);
                Gizmos.DrawRay(hit.point, hit.normal);
            }
        }
    }
}
