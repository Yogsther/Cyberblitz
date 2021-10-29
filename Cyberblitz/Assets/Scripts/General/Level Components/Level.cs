using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Level : MonoBehaviour
{
	public Color gridGizmoColor;
	public Vector2Int levelGridSize;
	public SpawnArea[] spawnAreas;
	public GridCollider[] gridColliders;

	public List<LevelElement> levelElements = new List<LevelElement>();

	public GameObject groundPrefab;
	private GameObject groundInstance;

	private Transform visualsParent;


	public bool showGameObjectsInEditor;
	[HideInInspector] public bool isShowingGameObjects;

	public void SetupLevel()
	{
		UpdateLevelElements();



		BoxCollider groundCollider = new GameObject("Ground").AddComponent<BoxCollider>();

		groundCollider.center = (levelGridSize - Vector2.one).ToFlatVector3() * .5f;
		groundCollider.size = levelGridSize.ToFlatVector3();

		groundCollider.transform.parent = transform;
		groundCollider.gameObject.layer = 6; // Ground
		foreach (LevelElement element in levelElements)
		{
			Transform elementTransform = element.Spawn(transform).transform;
		}

		if (LevelManager.instance.showGameObjects)
		{
			ShowElementGameObjects();
		}
	}

	public void ShowElementGameObjects()
	{
		Debug.Log("show");
		if (visualsParent == null)
		{
			GameObject visualsContainer = new GameObject("Level Visuals");

			visualsParent = visualsContainer.transform;

			if (visualsParent.parent != transform) visualsParent.parent = transform;
		}

		if (visualsParent != null)
		{
			if (groundPrefab != null)
			{
				groundInstance = Instantiate(groundPrefab, (levelGridSize.ToVector2() * .5f - Vector2.one * .5f).ToFlatVector3(), Quaternion.identity, visualsParent);
			}

			foreach (LevelElement element in levelElements)
			{
				if (element.gameObjectPrefab != null)
				{
					element.gameObjectInstance = Instantiate(element.gameObjectPrefab, element.center.ToFlatVector3(), Quaternion.identity, visualsParent);
				}
			}
			isShowingGameObjects = true;
		}
	}

	public void HideElementGameObjects()
	{

		if (visualsParent != null)
		{
			DestroyImmediate(visualsParent.gameObject);
		}

		if (groundInstance != null)
		{
			DestroyImmediate(groundInstance);
			groundInstance = null;
		}

		foreach (LevelElement element in levelElements)
		{
			if (element.gameObjectInstance != null)
			{
				DestroyImmediate(element.gameObjectInstance);
				element.gameObjectInstance = null;
			}
		}
		isShowingGameObjects = false;
	}

	public void UpdateElementGameObjects()
	{
		if (isShowingGameObjects)
		{
			foreach (LevelElement element in levelElements)
			{
				if (element.gameObjectInstance != null)
				{
					element.gameObjectInstance.transform.position = element.center.ToFlatVector3();
				}
			}
		}
	}

	public void UpdateLevelElements()
	{
		levelElements = new List<LevelElement>();

		levelElements.AddRange(spawnAreas);
		levelElements.AddRange(gridColliders);

		UpdateElementGameObjects();
	}


	private void OnValidate()
	{
		if (spawnAreas.Length + gridColliders.Length != levelElements.Count)
		{
			UpdateLevelElements();
		}
	}

	private void OnDrawGizmos()
	{
#if UNITY_EDITOR
		Handles.Label(Vector3.right * levelGridSize.x * .5f, $"W: {levelGridSize.x}");
		Handles.Label(Vector3.forward * levelGridSize.y * .5f, $"H: {levelGridSize.y}");
#endif
		DrawGizmosGrid();

		foreach (LevelElement element in levelElements) element.DrawGizmo();
	}

	private void DrawGizmosGrid()
	{
		Gizmos.color = gridGizmoColor;

		for (float y = -.5f; y < levelGridSize.y + .5f; y++)
		{
			float x1 = -.5f;
			float x2 = levelGridSize.x - .5f;

			Vector3 fromPoint = new Vector2(x1, y).ToFlatVector3();
			Vector3 toPoint = new Vector2(x2, y).ToFlatVector3();

			Gizmos.DrawLine(fromPoint, toPoint);
		}

		for (float x = -.5f; x < levelGridSize.x + .5f; x++)
		{
			float y1 = -.5f;
			float y2 = levelGridSize.y - .5f;

			Vector3 fromPoint = new Vector2(x, y1).ToFlatVector3();
			Vector3 toPoint = new Vector2(x, y2).ToFlatVector3();

			Gizmos.DrawLine(fromPoint, toPoint);
		}
	}
}
