using System;
using System.Collections.Generic;
using UnityEngine;

public class Timeline
{
	public UnitID ownerId;
	public List<Block> blocks = new List<Block>();

	/// <returns>The total duration of all blocks in the timeline</returns>
	public float GetDuration()
	{
		float totalDuration = 0f;
		foreach (Block block in blocks) totalDuration += block.duration;

		return totalDuration;
	}

	public int GetSize()
	{
		return blocks.Count;
	}

	/// <summary>
	/// Finds the last MoveBlock in the timeline and returns it's end position.
	/// </summary>
	/// <returns></returns>
	public Vector2 GetEndPosition(Vector2 fallback = new Vector2())
	{
		for (int i = blocks.Count - 1; i >= 0; i--)
		{
			if (blocks[i] is MoveBlock)
			{
				MoveBlock moveBlock = blocks[i] as MoveBlock;

				return moveBlock.GetEndPosition();
			}
		}

		Debug.LogWarning($"Could not get end position of timeline. Returned fallback Vector: {fallback}");

		return fallback;
	}

	public GridPoint GetOriginPoint(int index)
	{
		for (int i = index - 1; i >= 0; i--)
		{
			if (blocks[i] is MoveBlock)
			{
				MoveBlock moveBlock = blocks[i] as MoveBlock;

				return moveBlock.movementPath.target;
			}
		}

		return new GridPoint(MatchManager.match.GetUnit(ownerId).position.ToVector2().RoundToVector2Int());
	}

	public Vector2 GetPositionAtTime(float time)
	{
		for (int i = GetBlockAtTime(time).timelineIndex; i > 0; i--)
		{
			if (blocks[i] is MoveBlock)
			{
				MoveBlock moveBlock = blocks[i] as MoveBlock;

				float blockLocalTime = time - GetStartTimeOfBlock(moveBlock);

				return moveBlock.GetPositionAtTime(blockLocalTime);
			}
		}

		Vector2 fallback = Vector2.zero;

		Debug.LogWarning($"Could not get end position of timeline. Returned fallback Vector: {fallback}");

		return fallback;
	}

	/// <summary>
	/// Finds the Block by checking if <paramref name="time"/> is between the start and end times of the block.
	/// </summary>
	/// <returns>The active block at <paramref name="time"/>.</returns>
	public Block GetBlockAtTime(float time)
	{
		float clampedTime = Mathf.Clamp(time, 0f, GetDuration());

		foreach (Block block in blocks)
		{
			float startTime = GetStartTimeOfBlock(block);
			float endTime = GetEndTimeOfBlock(block);

			if (clampedTime >= startTime && clampedTime <= endTime)
			{
				return block;
			}
		}

		/*Debug.LogWarning("No block found at " + time);*/

		return null;
	}

	public bool TryGetBlockAtTime(float time, out Block outputBlock)
	{
		outputBlock = null;

		foreach (Block block in blocks)
		{
			float startTime = GetStartTimeOfBlock(block);
			float endTime = GetEndTimeOfBlock(block);

			if (startTime <= time && time <= endTime)
			{
				outputBlock = block;
				return true;
			}
		}

		/*Debug.LogWarning("No block found at " + time);*/

		return false;
	}

	public Block GetBlock(BlockID id)
	{
		foreach (Block block in blocks)
		{
			if (block.id == id) return block;
		}
		return null;
	}

	public Block GetBlockAtIndex(int index)
	{
		return blocks[index];
	}

	public Block GetLastBlock()
	{
		return blocks[blocks.Count - 1];
	}

	/// <summary>
	/// Gets the start time of <see cref="blocks"/>[<paramref name="index"/>] by adding together the <see cref="Block.duration"/>
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
	public float GetStartTimeOfBlockAtIndex(int index)
	{
		float totalDurationOfPreviousBlocks = 0f;

		for (int i = 0; i < index; i++)
		{
			totalDurationOfPreviousBlocks += blocks[i].duration;
		}

		return totalDurationOfPreviousBlocks;
	}

	public float GetStartTimeOfBlock(Block block)
	{
		return GetStartTimeOfBlockAtIndex(block.timelineIndex);
	}

	public float GetEndTimeOfBlock(Block block)
	{
		return GetStartTimeOfBlock(block) + block.duration;
	}

	/// <summary>
	/// Updates <see cref="Block.timelineIndex"/> of all <see cref="Block"/>s in <see cref="blocks"/> to match with their current index
	/// </summary>
	public void UpdateBlockIndexes()
	{
		for (int i = 0; i < blocks.Count; i++)
		{
			blocks[i].timelineIndex = i;
		}
	}

	/// <summary>
	/// Inserts <paramref name="block"/> at <see cref="blocks"/>[<paramref name="index"/>] and calls <see cref="UpdateBlockIndexes"/>
	/// </summary>
	public void InsertBlock(Block block, int index = -1)
	{
		if (index == -1 || index > GetSize())
		{
			block.timelineIndex = blocks.Count;
			blocks.Add(block);
		} else
		{
			blocks.Insert(index, block);
			block.timelineIndex = index;
		}

		UpdateBlockIndexes();
	}

	/// <summary>
	/// Removes the <see cref="Block"/> at <see cref="blocks"/>[<paramref name="index"/>] and calls <see cref="UpdateBlockIndexes"/>
	/// </summary>
	public void RemoveBlockAtIndex(int index)
	{
		blocks.RemoveAt(index);

		UpdateBlockIndexes();
	}

	public void RemoveBlock(Block block)
	{
		RemoveBlockAtIndex(block.timelineIndex);

		UpdateBlockIndexes();
	}

	public void Clear()
	{
		blocks.Clear();
	}

}