using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyCamera : MonoBehaviour
{
	public Quaternion cameraUpAngle;
	public Quaternion cameraDownAngle;
	public Camera lobbyCamera;


	public float animationSmoothingIn, animationSmoothingOut;
	Vector3 smoothCameraVel;


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
