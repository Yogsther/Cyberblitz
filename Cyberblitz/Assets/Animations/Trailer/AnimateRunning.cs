using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateRunning : MonoBehaviour
{
	public bool animate = false;
	public float speed;

	void Start()
	{

	}

	void Update()
	{
		if (animate) transform.position += transform.right * speed * Time.deltaTime;
	}
}
