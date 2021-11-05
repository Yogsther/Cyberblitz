using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager instance { get; private set; }

	public GameplayUIManager GameplayUIManager;

	public TimelineEditor TimelineEditor;
	public CameraController CameraController;

	public LayerMask blockPathfinderMask;

    private void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(gameObject);
		} else
		{
			instance = this;
		}

		LevelManager.OnLevelLoaded += CameraController.InitCamera;
	}
}
