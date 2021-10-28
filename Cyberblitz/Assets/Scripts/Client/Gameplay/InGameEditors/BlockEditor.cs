using System;
using System.Collections;
using System.Collections.Generic;
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

		yield return null;

		Debug.Log("[BlockEditor] - Started editing a block");

		switch (block.type)
		{
			case BlockType.Move:
			{

				MoveBlock moveBlock = block as MoveBlock;

				if (moveBlock.movementPath == null)
				{
					moveBlock.movementPath = new AutoGridPath(MatchManager.match.GetUnit(moveBlock.ownerId).timeline.GetOriginPoint(moveBlock.timelineIndex));
				}



				pathEditor.EditGridPath(ref moveBlock.movementPath, 500f + currentFreeTime);
				pathEditor.OnUpdated += UpdateBlock;


				while (selectedBlock == block)
				{
					yield return null;
				}


				pathEditor.StopEditing();

				pathEditor.OnUpdated -= UpdateBlock;
				/*}*//*else
				{
					Debug.LogWarning("MoveBlock.movementPath was null");
				}*/
			}
			break;
			case BlockType.Guard:
			{
				GuardBlock guardBlock = block as GuardBlock;

				if (guardBlock.aimCone != null)
				{
					visionConeEditor.EditGridPath(ref guardBlock.aimCone);
					visionConeEditor.OnUpdated += UpdateBlock;

					while (selectedBlock == block)
					{
						yield return null;
					}

					visionConeEditor.OnUpdated -= UpdateBlock;
				} else
				{
					Debug.LogWarning("GuardBlock.aimCone was null");
				}
			}
			break;
		}

		Debug.Log("[BlockEditor] - Stopped editing a block");

	}

	public void UpdateBlock()
	{
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
