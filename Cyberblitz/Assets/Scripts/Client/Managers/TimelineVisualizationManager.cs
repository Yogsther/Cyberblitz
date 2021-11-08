using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TimelineVisualizationManager : MonoBehaviour
{

	public LineRenderer moveBlockLine, tempCone, lockedConePrefab;
	public MoveBlockVisual moveBlockVisualPrefab;
	public Waypoint waypointPrefab;
	public Camera camera;
	public Transform blockVisualsParent, waypointParent, lockedConesParent;

	Unit selectedUnit;
	Block selectedBlock;

	List<BlockVisual> blockVisuals = new List<BlockVisual>();

	List<Waypoint> waypoints = new List<Waypoint>();

	void Awake()
	{
		TimelineEditor.OnBlockUpdate += OnBlockUpdated;
		TimelineEditor.OnUnitSelected += OnUnitSelected;
		TimelineEditor.OnBlockSelected += OnBlockSelected;
		TimelineEditor.OnBlockDeselected += OnBlockDeselected;
		MatchManager.OnMatchUpdate += OnMatchUpdate;
	}

	void OnBlockDeselected()
	{
		selectedBlock = null;
		OnBlockUpdated();
	}

	void OnMatchUpdate(Match match)
	{
		if (match.state != Match.GameState.Planning)
		{
			selectedBlock = null;
			ClearVisualElements();
		}
	}

	void CreateNewWaypoint(MoveBlock block)
	{
		if (block.movementPath == null) return;
		Waypoint waypoint = Instantiate(waypointPrefab, waypointParent);
		waypoint.transform.position = block.movementPath.target.point.ToFlatVector3(waypoint.transform.position.y);
		waypoint.index = waypoints.Count;
		waypoint.block = block;
		waypoint.SetCamera(camera);
		waypoint.SetSelected(selectedBlock == block);

		waypoints.Add(waypoint);
	}

	void ClearVisualElements()
	{
		tempCone.positionCount = 0;
		//moveBlockLine.positionCount = 0;
		ClearTransform(blockVisualsParent);
		ClearTransform(waypointParent);
		ClearTransform(lockedConesParent);

		blockVisuals.Clear();
		waypoints.Clear();
	}

	void ClearTransform(Transform parent)
	{
		foreach (Transform child in parent)
		{
			Destroy(child.gameObject);
		}
	}

	void CreateNewLockedCone(Vector2[] points)
	{
		LineRenderer line = Instantiate(lockedConePrefab, lockedConesParent);
		DrawCone(points, line);
	}

	void DrawCone(Vector2[] points, LineRenderer line)
	{
		float height = .2f;
		int coneResolution = 15;

		line.positionCount = points.Length + 1; // Add one for the closing point at the end
		line.positionCount += coneResolution;

		// Draw the first two positions, from unit position to the outer left point of the cone
		for (int i = 0; i < 2; i++)
		{
			line.SetPosition(i, points[i].ToFlatVector3(height));
		}

		Vector2 startDiration = points[1] - points[0];
		Vector2 endDirection = points[2] - points[0];
		float startAngle = Mathf.Atan2(startDiration.y, startDiration.x);
		float endAngle = Mathf.Atan2(endDirection.y, endDirection.x);

		float coneDistance = Vector2.Distance(points[0], points[1]);

		// Angle step is the resolution of the curved part of the cone
		float angleStep = (endAngle - startAngle) / coneResolution;

		for (int i = 0; i < coneResolution; i++)
		{
			float angle = Mathf.LerpAngle(startAngle * Mathf.Rad2Deg, endAngle * Mathf.Rad2Deg, (float)i / (float)coneResolution) * Mathf.Deg2Rad;
			float x = (Mathf.Cos(angle) * coneDistance) + points[0].x;
			float y = (Mathf.Sin(angle) * coneDistance) + points[0].y;
			line.SetPosition(i + 2, new Vector2(x, y).ToFlatVector3(height));
		}

		line.SetPosition(line.positionCount - 2, points[2].ToFlatVector3(height));
		line.SetPosition(line.positionCount - 1, points[0].ToFlatVector3(height));
	}


	void LoadMoveBlock(MoveBlock block)
	{
		MoveBlockVisual moveBlockVisual = Instantiate(moveBlockVisualPrefab, blockVisualsParent);
		moveBlockVisual.block = block;

		moveBlockVisual.Init();

		blockVisuals.Add(moveBlockVisual);

		//CreateNewWaypoint(block);
	}

	void OnUnitSelected(UnitID id)
	{
		selectedUnit = MatchManager.GetUnit(id);
		OnBlockUpdated();
	}

	void OnBlockSelected(Block block)
	{
		selectedBlock = block;
		OnBlockUpdated(block);
	}

	void OnBlockUpdated(Block editedBlock = null)
	{
		//ClearVisualElements();
		if (selectedUnit == null) return;
		foreach (Block block in selectedUnit.timeline.blocks)
		{
			if (!GetTrackedBlockIds().Contains(block.id))
			{
				if (block.type == BlockType.Move)
					LoadMoveBlock((MoveBlock)block);
				if (block.type == BlockType.Guard)
				{
					GuardBlock guardBlock = (GuardBlock)block;
					if (guardBlock.aimCone != null && guardBlock.aimCone.isSet)
					{
						Vector2[] points = guardBlock.aimCone.GetConePoints(guardBlock.aimCone.direction);
						CreateNewLockedCone(points);
					}
				}
			}
		}

		foreach(BlockVisual blockVisual in GetUnitBlockVisuals(selectedUnit))
        {
			blockVisual.UpdateVisuals();
        }
	}

	private List<BlockID> GetTrackedBlockIds()
    {
		List<BlockID> trackedIds = new List<BlockID>();

		foreach(BlockVisual blockVisual in blockVisuals)
        {
			trackedIds.Add(blockVisual.block.id);
        }

		return trackedIds;
    }

	List<BlockVisual> GetUnitBlockVisuals(Unit unit)
    {
		List<BlockVisual> unitBlockVisuals = new List<BlockVisual>();

		foreach(BlockVisual blockVisual in blockVisuals)
        {
			if (blockVisual.block.ownerId == unit.id) unitBlockVisuals.Add(blockVisual);
        }

		return unitBlockVisuals;
    }

	private void Update()
	{
		if (selectedBlock != null)
		{
			if (selectedBlock.type == BlockType.Guard)
			{
				GuardBlock guardBlock = (GuardBlock)selectedBlock;
				float inputDirection = 0;
				if (!InputManager.isOnGui && InputManager.TryGetPointerHitLayer(LayerMask.GetMask("Ground"), out RaycastHit groundHit))
				{

					Vector2 mouseHitPoint = groundHit.point.FlatVector3ToVector2();

					Vector2 toMouse = mouseHitPoint - guardBlock.aimCone.origin.point;

					inputDirection = (Mathf.Atan2(toMouse.y, toMouse.x) * Mathf.Rad2Deg) - 90f;

					Vector2[] points = guardBlock.aimCone.GetConePoints(inputDirection);
					DrawCone(points, tempCone);
				} else
				{
					tempCone.positionCount = 0;
				}
			}

		}

	}
}
