using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneAnimation : MonoBehaviour
{

	public Transform crane;
	float nextAnimationStart = 0;
	Vector3 animationTarget;
	Vector3 dampVal;
	bool animating = false;

	public float speed;

	void Update()
	{
		crane.Rotate(crane.up, speed * Time.deltaTime);
	}

}
