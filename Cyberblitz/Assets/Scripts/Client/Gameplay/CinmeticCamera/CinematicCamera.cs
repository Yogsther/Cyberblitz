using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicCamera : MonoBehaviour
{
	public float dollySpeed = 1f;

	public new Camera camera;
	public AudioListener microphone;
	public CinemachineSmoothPath circlePath;
	public CinemachineVirtualCamera virtualCamera;
	CinemachineTrackedDolly dolly;

	void Start()
	{
		dolly = virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
		StartCircle();
	}

	public void StartCircle()
	{
		dolly.m_Path = circlePath;
		StartCoroutine(CircleEnumerator());

	}

	IEnumerator CircleEnumerator()
	{

		while (true)
		{
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
