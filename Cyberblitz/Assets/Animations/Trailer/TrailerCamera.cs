using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailerCamera : MonoBehaviour
{
	bool animating = false;
	public float speed;
	CinemachineTrackedDolly dolly;
	public CinemachineVirtualCamera virtualCamera;

	[ContextMenu("Animate!")]
	public void Animate()
	{
		dolly.m_PathPosition = 0f;
		animating = false;
		StartCoroutine(AnimateEnumerator());
	}

	public IEnumerator AnimateEnumerator()
	{
		animating = true;
		while (animating)
		{
			dolly.m_PathPosition += Time.deltaTime * speed;
			yield return new WaitForEndOfFrame();
		}
	}

	void Start()
	{
		dolly = virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
	}

	void Update()
	{

	}
}
