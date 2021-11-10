using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyCamera : MonoBehaviour
{
	public Quaternion cameraUpAngle;
	public Quaternion cameraDownAngle;
	public Camera lobbyCamera;

	public Transform menuPosition, loginPosition;


	public float animationSmoothingIn, animationSmoothingOut;
	Vector3 smoothCameraVel;
	Vector3 cameraPositionVel;

	private void Start()
	{
		lobbyCamera.transform.position = loginPosition.position;
		lobbyCamera.transform.rotation = loginPosition.rotation;
	}

	public void AnimateIn(Action callback)
	{
		lobbyCamera.transform.rotation = cameraUpAngle;
		StartCoroutine(AnimateCamera(cameraDownAngle, animationSmoothingIn, callback));
	}

	public void AnimateOut(Action callback)
	{
		lobbyCamera.transform.rotation = cameraDownAngle;
		StartCoroutine(AnimateCamera(cameraUpAngle, animationSmoothingOut, callback));
	}

	public void AnimateFromLogin(Action callback)
	{
		StartCoroutine(AnimateTo(menuPosition, .1f, callback));
	}

	IEnumerator AnimateTo(Transform target, float smoothing, Action callback = null)
	{
		while (Vector3.Distance(target.position, lobbyCamera.transform.position) > .001f)
		{
			lobbyCamera.transform.position = Vector3.SmoothDamp(lobbyCamera.transform.position, target.position, ref cameraPositionVel, smoothing);
			lobbyCamera.transform.rotation = SmoothDampQuaternion(lobbyCamera.transform.rotation, target.rotation, ref smoothCameraVel, smoothing);

			yield return new WaitForEndOfFrame();
		}
		if (callback != null) callback();
	}

	IEnumerator AnimateCamera(Quaternion target, float smoothing, Action callback = null)
	{
		while (Quaternion.Angle(target, lobbyCamera.transform.rotation) > .5f)
		{
			lobbyCamera.transform.rotation = SmoothDampQuaternion(lobbyCamera.transform.rotation, target, ref smoothCameraVel, smoothing);
			yield return new WaitForEndOfFrame();
		}
		if (callback != null) callback();
	}

	public static Quaternion SmoothDampQuaternion(Quaternion current, Quaternion target, ref Vector3 currentVelocity, float smoothTime)
	{
		Vector3 c = current.eulerAngles;
		Vector3 t = target.eulerAngles;
		return Quaternion.Euler(
		  Mathf.SmoothDampAngle(c.x, t.x, ref currentVelocity.x, smoothTime),
		  Mathf.SmoothDampAngle(c.y, t.y, ref currentVelocity.y, smoothTime),
		  Mathf.SmoothDampAngle(c.z, t.z, ref currentVelocity.z, smoothTime)
		);
	}

}
