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

	public static Ray pointerRay;

	public LayerMask pointerLayers;

	public Camera mainCamera;



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


		if (mainCamera != null) PerformPointerRaycast();
	}

	private void PerformPointerRaycast()
	{

		pointerRay = mainCamera.ScreenPointToRay(pointerScreenPosition);

		pointerHit = Physics.Raycast(pointerRay, out pointerHitInfo, 100f, pointerLayers);
		pointerHitInfos = Physics.RaycastAll(pointerRay, 100f, pointerLayers);

	}

	public static bool TryGetPointerHitLayer(LayerMask layerMask, out RaycastHit raycastHit)
	{
		if (pointerHitInfos != null)
		{

			foreach (RaycastHit hit in pointerHitInfos)
			{
				if (hit.transform != null)
				{
					bool hitObjectIsInLayerMask = layerMask == (layerMask | (1 << hit.transform.gameObject.layer));

					if (hitObjectIsInLayerMask)
					{
						raycastHit = hit;

						return true;
					}
				}
			}
		}

		raycastHit = new RaycastHit();

		return false;
	}

	public static bool TryGetPointerHitLayer(LayerMask targetLayers, LayerMask blockingLayers, out RaycastHit raycastHit)
    {
		raycastHit = new RaycastHit();

		if (pointerHitInfos != null)
		{

			foreach (RaycastHit hit in pointerHitInfos)
			{
				bool hitObjectIsBlocking = blockingLayers == (blockingLayers | (1 << hit.collider.gameObject.layer));

				bool hitObjectIsTarget = targetLayers == (targetLayers | (1 << hit.collider.gameObject.layer));

				if (hitObjectIsTarget)
				{
					raycastHit = hit;

					return true;
				}

                if (hitObjectIsBlocking)
                {
					return false;
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
