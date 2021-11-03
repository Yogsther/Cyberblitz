using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCamera : MonoBehaviour
{
	public enum ActionCameraType
	{
		Weapon,
		Helmet
	}
	public ActionCameraType type;
	public Camera camera;
	public AudioListener microphone;
}
