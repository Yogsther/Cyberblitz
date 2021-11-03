using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CinematicCamera : MonoBehaviour
{
	public float dollySpeed = 1f;
	public float flyInSpeed = 3f;

	public Transform focusPoint;
	public new Camera camera;
	public Camera planningCamera;
	public AudioListener microphone;
	public CinemachineSmoothPath circlePath, zoomPath;
	public CinemachineVirtualCamera virtualCamera;
	CinemachineTrackedDolly dolly;

	public Transform focusedUnit;

	bool circling = false;

	float unitHeight = 1.7f;
	float flyinHeight = 10f;
	float rightOfCharacterAmount = 8f;
	float flyBehindCharacterAmount = 4f;
	float flyBehindRightOffset = 2.5f;
	float lookAheadFocus = 5f;
	float flyBehindHeight = 2f;
	float flyinProgressStop = 8f;
	Vector3 flyinTarget;

	ActionCamera actionCamera;

	public Image[] blinds;
	float blindHeight = 540f;
	float blindOpenSpeed = 1500f;

	void SetBlindsVisible(bool show)
	{
		SetBlindHeights(show ? blindHeight : 0);
	}

	void SetBlindHeights(float height)
	{
		foreach (Image blind in blinds)
			blind.rectTransform.sizeDelta = new Vector2(blind.rectTransform.sizeDelta.x, height);
	}

	void Start()
	{
		dolly = virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
		SetBlindsVisible(false);
		EnablePlanningCamera();
	}

	void EnablePlanningCamera()
	{
		DisableActionCamera();
		camera.enabled = false;
		planningCamera.enabled = true;
	}

	void EnableCinematicCamera()
	{
		DisableActionCamera();
		planningCamera.enabled = false;
		camera.enabled = true;
	}

	void DisableActionCamera()
	{
		if (actionCamera != null) actionCamera.camera.enabled = false;
	}



	[ContextMenu("Create zoom path")]
	void CreateZoomPath(Transform unit)
	{

		actionCamera = unit.GetComponentInChildren<ActionCamera>();

		if (actionCamera == null)
		{
			Debug.LogWarning("No action camera found on unit");
			StartCircling();
			return;
		}

		List<CinemachineSmoothPath.Waypoint> path = new List<CinemachineSmoothPath.Waypoint>();
		flyinTarget = GetUnitHeadPosition();

		path.Add(CreateWaypoint(camera.transform.position));

		Vector3 flyinRight = flyinTarget + (unit.right * rightOfCharacterAmount) + (unit.up * flyinHeight);
		path.Add(CreateWaypoint(flyinRight));

		Vector3 flyBehind = flyinTarget + (unit.forward * -flyBehindCharacterAmount) + (unit.right * flyBehindRightOffset) + (unit.up * flyBehindHeight);
		path.Add(CreateWaypoint(flyBehind));

		Vector3 flyToUnit = flyinTarget;

		path.Add(CreateWaypoint(flyToUnit));

		zoomPath.m_Waypoints = path.ToArray();
		virtualCamera.LookAt = focusPoint;

		circling = false;
		dolly.m_Path = zoomPath;
		dolly.m_PathPosition = 0f;


		StartCoroutine(StartFlyingIn());
	}

	Vector3 GetUnitHeadPosition()
	{
		return new Vector3(focusedUnit.position.x, unitHeight, focusedUnit.position.z);
	}

	IEnumerator StartFlyingIn()
	{
		while (dolly.m_PathPosition <= flyinProgressStop)
		{
			Vector3 flyinFocus = (focusedUnit.forward * lookAheadFocus) + GetUnitHeadPosition();
			focusPoint.position = flyinFocus;
			dolly.m_PathPosition += Time.deltaTime * flyInSpeed;


			yield return new WaitForEndOfFrame();
		}
		SwitchToActionCamera();
	}

	void SwitchToActionCamera()
	{
		SetBlindsVisible(true);
		camera.enabled = false;
		planningCamera.enabled = false;
		actionCamera.camera.enabled = true;
		StartCoroutine(AnimateUpBlinds());
	}

	IEnumerator AnimateUpBlinds()
	{
		float height = blindHeight;
		while (height > 0)
		{
			height -= blindOpenSpeed * Time.deltaTime;
			SetBlindHeights(height);
			yield return new WaitForEndOfFrame();
		}
	}


	CinemachineSmoothPath.Waypoint CreateWaypoint(Vector3 position)
	{
		CinemachineSmoothPath.Waypoint waypoint = new CinemachineSmoothPath.Waypoint();
		waypoint.position = position;
		return waypoint;
	}

	[ContextMenu("Start circle")]
	public void StartCircling()
	{

		if (actionCamera != null) actionCamera.camera.enabled = false;
		camera.enabled = true;

		circling = true;
		dolly.m_Path = circlePath;
		dolly.m_PathPosition = 0f;
		virtualCamera.LookAt = focusPoint;

		StartCoroutine(CircleEnumerator());

	}

	IEnumerator CircleEnumerator()
	{

		while (circling)
		{
			focusPoint.position = focusedUnit.position;
			dolly.m_PathPosition += Time.deltaTime * dollySpeed;
			yield return new WaitForEndOfFrame();
		}
	}

	/*public Camera cinemaCam;
	bool circlingAroundMap = false;
	public float circleSpeed;
	Vector3 circleFocus;
	Vector3 circlePivot;
	Vector3 lastCirclePosition;

	Vector3 desiredCircleFocus;
	Vector3 circleFocusSmoothingVal = Vector3.zero;

	public Transform focusTransform, pivotTransform, startTransform;

	void Start()
	{
		transform.position = startTransform.position;
		circleFocus = focusTransform.position;
		circlePivot = pivotTransform.position;
		*//*BeginCircling();*//*
	}


	void BeginCircling()
	{
		circlingAroundMap = true;
		StartCoroutine(CircleRoutine());
	}

	[ContextMenu("Start transition to Action Cam")]
	public void TransferToActionCamera()
	{
		circlingAroundMap = false;
		lastCirclePosition = transform.position;
		StartCoroutine(FlyToUnit());
	}

	IEnumerator FlyToUnit()
	{
		// The amount if distance infront of the unit the camera will look when switching focus
		float forwardDistanceLook = 10f;
		// The height of a unit to align camera height
		float unitHeight = 1.73f;

		Transform targetUnit = focusTransform;

		ActionCamera actionCamera = targetUnit.GetComponentInChildren<ActionCamera>();

		Vector3 targetPosition = new Vector3(targetUnit.position.x, unitHeight, targetUnit.position.z);
		Vector3 targetSight = targetPosition + targetUnit.forward * forwardDistanceLook;
		Vector3 descendVel = Vector3.zero;

		// The amount of distance from the unit when the camera will cut over to the action camera.
		float stopDistance = .5f;

		// The distance from the unit when the camera will switch focus from the unit to where to unit is looking.
		float switchFocusDistance = 10f;

		bool hasSwitchedFocus = false;
		float flyFocusSwitchSmoothing = .8f;
		Vector3 flyFocus = targetPosition;
		Vector3 desiredFlyFocus = flyFocus;
		Vector3 flyFocusSmoothingVel = Vector3.zero;

		int frames = 0;
		while (true)
		{
			frames++;
			flyFocus = Vector3.SmoothDamp(flyFocus, desiredFlyFocus, ref flyFocusSmoothingVel, flyFocusSwitchSmoothing);

			transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref descendVel, .5f);
			transform.LookAt(flyFocus);

			float distance = Vector3.Distance(transform.position, targetPosition);

			if (!hasSwitchedFocus && distance < switchFocusDistance)
			{
				Debug.Log("Switched focus at frame " + frames);
				hasSwitchedFocus = true;
				desiredFlyFocus = targetSight;
			}

			if (distance < stopDistance)
			{
				Debug.Log("Done at frame: " + frames);
				break;
			}

			yield return new WaitForEndOfFrame();
		}
		Debug.Log("DONE!");
	}


	IEnumerator CircleRoutine()
	{
		while (circlingAroundMap)
		{
			desiredCircleFocus = focusTransform.position;
			circleFocus = Vector3.SmoothDamp(circleFocus, desiredCircleFocus, ref circleFocusSmoothingVal, 1f);

			cinemaCam.transform.LookAt(circlePivot);
			transform.Translate(Vector3.right * Time.deltaTime * circleSpeed);
			cinemaCam.transform.LookAt(circleFocus);
			yield return new WaitForEndOfFrame();
		}
	}*/



	/*void PlayScene(Transform start, Transform end, Transform focus, float duration)
	{
		StartCoroutine(PlaySceneRoutine(start, end, focus, duration));
	}

	IEnumerator PlaySceneRoutine(Transform start, Transform end, Transform focus, float duration)
	{
		float startTime = Time.time;
		Vector3 smoothVal = Vector3.zero;

		while (Time.time - startTime < startTime + duration)
		{
			float timeLeft = duration - (Time.time - startTime);

			cinemaCam.transform.position = Vector3.SmoothDamp(cinemaCam.transform.position, end.transform.position, ref smoothVal, timeLeft);
			cinemaCam.transform.LookAt(focus);

			yield return new WaitForEndOfFrame();
		}
	}*/

	void Update()
	{

	}
}
