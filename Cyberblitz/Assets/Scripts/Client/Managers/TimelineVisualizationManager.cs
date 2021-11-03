using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TimelineVisualizationManager : MonoBehaviour
{

	public LineRenderer moveBlockLine, tempCone, lockedConePrefab;
	public Waypoint waypointPrefab;
	public Camera camera;
	public Transform waypointParent, lockedConesParent;

	Unit selectedUnit;
	Block selectedBlock;

	List<Waypoint> waypoints = new List<Waypoint>();

	void Start()
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

	void DrawPath(Unit unit)
	{
		Timeline timeline = unit.timeline;
		float lineHeight = 0.21f;

		// Set the first position to current unit position
		moveBlockLine.positionCount = 1;
		moveBlockLine.SetPosition(0, unit.position.ToVector2().ToFlatVector3(lineHeight));

		foreach (Block block in timeline.blocks)
		{
			if (block.type == BlockType.Move)
			{
				MoveBlock moveBlock = (MoveBlock)block;
				if (moveBlock.movementPath != null)
				{
					foreach (Vector2Int point in moveBlock.movementPath.GetPoints())
					{
						// Add line point for movement block target
						moveBlockLine.positionCount++;
						moveBlockLine.SetPosition(moveBlockLine.positionCount - 1, point.ToFlatVector3(lineHeight));
					}
				}


			}
		}

		// If no movementblocks were added, remove the line.
		if (moveBlockLine.positionCount == 1) moveBlockLine.positionCount = 0;
	}

	void ClearVisualElements()
	{
		tempCone.positionCount = 0;
		moveBlockLine.positionCount = 0;

		ClearTransform(waypointParent);
		ClearTransform(lockedConesParent);

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

		// Angle step is the resolution of the curved part of the cone
		float angleStep = (endAngle - startAngle) / coneResolution;

		for (int i = 0; i < coneResolution; i++)
		{
			float angle = Mathf.LerpAngle(startAngle * Mathf.Rad2Deg, endAngle * Mathf.Rad2Deg, (float)i / (float)coneResolution) * Mathf.Deg2Rad;
			float x = (Mathf.Cos(angle) * 5f) + points[0].x;
			float y = (Mathf.Sin(angle) * 5f) + points[0].y;
			line.SetPosition(i + 2, new Vector2(x, y).ToFlatVector3(height));
		}

		line.SetPosition(line.positionCount - 2, points[2].ToFlatVector3(height));
		line.SetPosition(line.positionCount - 1, points[0].ToFlatVector3(height));
	}


	void LoadMoveBlock(MoveBlock block)
	{
		CreateNewWaypoint(block);
	}

	void OnUnitSelected(Unit unit)
	{
		selectedUnit = unit;
		OnBlockUpdated();
	}

	void OnBlockSelected(Block block)
	{
		selectedBlock = block;
		OnBlockUpdated(block);
	}

	void OnBlockUpdated(Block editedBlock = null)
	{
		ClearVisualElements();
		if (selectedUnit == null) return;
		foreach (Block block in selectedUnit.timeline.blocks)
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

		DrawPath(selectedUnit);
	}

	private void Update()
	{
		if (selectedBlock != null)
		{
			if (selectedBlock.type == BlockType.Guard)
			{
				GuardBlock guardBlock = (GuardBlock)selectedBlock;
				float inputDirection = 0;
				int groundLayer = 6;
				if (!InputManager.isOnGui && InputManager.TryGetPointerHitLayer(groundLayer, out RaycastHit groundHit))
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
