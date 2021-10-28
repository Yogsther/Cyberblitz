using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TimelineVisualizationManager : MonoBehaviour
{

	public LineRenderer line;
	public Waypoint waypointPrefab;
	public Camera camera;

	Unit selectedUnit;
	Block selectedBlock;

	List<Waypoint> waypoints = new List<Waypoint>();

	void Start()
	{
		TimelineEditor.OnBlockUpdate += OnBlockUpdated;
		TimelineEditor.OnUnitSelected += OnUnitSelected;
		TimelineEditor.OnBlockSelected += OnBlockSelected;
		MatchManager.OnMatchUpdate += OnMatchUpdate;
	}

	void OnMatchUpdate(Match match)
	{
		if (match.state != Match.GameState.Planning) ClearVisualElements();
	}

	void CreateNewWaypoint(MoveBlock block)
	{
		if (block.movementPath == null) return;
		Waypoint waypoint = Instantiate(waypointPrefab, transform);
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
		line.positionCount = 1;
		line.SetPosition(0, unit.position.ToVector2().ToFlatVector3(lineHeight));

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
						line.positionCount++;
						line.SetPosition(line.positionCount - 1, point.ToFlatVector3(lineHeight));
					}
				}


			}
		}

		// If no movementblocks were added, remove the line.
		if (line.positionCount == 1) line.positionCount = 0;
	}

	void ClearVisualElements()
	{
		line.positionCount = 0;
		foreach (Transform obj in transform)
		{
			Destroy(obj.gameObject);
		}
		waypoints.Clear();
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
		}

		DrawPath(selectedUnit);
	}
}
