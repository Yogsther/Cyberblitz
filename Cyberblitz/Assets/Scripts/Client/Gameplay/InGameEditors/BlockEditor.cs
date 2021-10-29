using System.Collections;
using UnityEngine;

public class BlockEditor : InGameEditor
{
	public AutoGridPathEditor pathEditor;
	public VisionConeEditor visionConeEditor;

	private Block selectedBlock;
	private float currentFreeTime;

	public void EditBlock(ref Block block, float freeTime)
	{
		currentFreeTime = freeTime;

		StartCoroutine(BlockEditing(block));
	}

	public void StopEditing()
	{
		selectedBlock = null;
	}

	public IEnumerator BlockEditing(Block block)
	{

		selectedBlock = block;

		Unit blockOwner = MatchManager.match.GetUnit(block.ownerId);
		UnitStats ownerStats = UnitDataManager.GetUnitDataByType(blockOwner.type).stats;

		GridPoint originPoint = blockOwner.timeline.GetOriginPoint(block.timelineIndex);

		yield return null;

		Debug.Log("[BlockEditor] - Started editing a block");

		switch (block.type)
		{
			case BlockType.Move:
			{

				MoveBlock moveBlock = block as MoveBlock;

				if (moveBlock.movementPath == null)
				{
					moveBlock.movementPath = new AutoGridPath(originPoint);
				}

				// TODO Remove 500f below, but it causes the game to crash.
				pathEditor.EditGridPath(ref moveBlock.movementPath, 500f + currentFreeTime);
				pathEditor.OnUpdated += UpdateBlock;


				while (selectedBlock == block)
				{
					yield return null;
				}


				pathEditor.StopEditing();

				pathEditor.OnUpdated -= UpdateBlock;

			}
			break;
			case BlockType.Guard:
			{
				GuardBlock guardBlock = block as GuardBlock;

				if (guardBlock.aimCone == null)
				{
					guardBlock.aimCone = new VisionCone(originPoint, ownerStats.range, ownerStats.spread);
				}

				visionConeEditor.EditVisionCone(ref guardBlock.aimCone);
				visionConeEditor.OnUpdated += UpdateBlock;

				while (selectedBlock == block)
				{
					yield return null;
				}

				visionConeEditor.OnUpdated -= UpdateBlock;
			}
			break;
		}

		Debug.Log("[BlockEditor] - Stopped editing a block");

	}

	public void UpdateBlock()
	{
		if (selectedBlock == null) return;
		Timeline blockTimeline = MatchManager.GetUnit(selectedBlock.ownerId).timeline;

		GridPoint lastGridPoint = null;

		foreach (Block block in blockTimeline.blocks)
		{

			switch (block.type)
			{
				case BlockType.Move:
				{
					MoveBlock moveBlock = block as MoveBlock;

					if (lastGridPoint != null)
					{
						moveBlock.movementPath.origin = lastGridPoint;
					}

					lastGridPoint = moveBlock.movementPath.target;

					moveBlock.duration = moveBlock.movementPath.GetTotalPathLength();

				}
				break;
				case BlockType.Guard:
					break;
			}

		}
		OnUpdated?.Invoke();
	}
}
