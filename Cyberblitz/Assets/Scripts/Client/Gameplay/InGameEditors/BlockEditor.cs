using System;
using System.Collections;
using UnityEngine;

public class BlockEditor : InGameEditor
{
    public AutoGridPathEditor pathEditor;
    public VisionConeEditor visionConeEditor;

    private Block selectedBlock;
    private UnitStats blockOwnerStats;
    private TimelineEditor timelineEditor;

    public static Action<BlockID> OnBlockSet;

    public void EditBlock(ref Block block, TimelineEditor editor)
    {
        timelineEditor = editor;

        StartCoroutine(BlockEditing(block));
    }

    public void StopEditing()
    {
        selectedBlock = null;
        pathEditor.StopEditing();
        visionConeEditor.StopEditing();
    }

    public IEnumerator BlockEditing(Block block)
    {
        if (block != selectedBlock)
        {

            selectedBlock = block;

            Unit blockOwner = MatchManager.match.GetUnit(block.ownerId);
            blockOwnerStats = UnitDataManager.GetUnitDataByType(blockOwner.type).stats;

            GridPoint originPoint = blockOwner.timeline.GetOriginPoint(block.timelineIndex);

            yield return null;

            Debug.Log($"[BlockEditor] - Started editing block {block.id}");

            switch (block.type)
            {
                case BlockType.Move:
                {

                    MoveBlock moveBlock = block as MoveBlock;

                    if (moveBlock.movementPath == null)
                    {
                        moveBlock.movementPath = new AutoGridPath(originPoint);

                        OnBlockSet?.Invoke(block.id);
                    }

                    // TODO Remove 500f below, but it causes the game to crash.
                    pathEditor.EditGridPath(ref moveBlock.movementPath);
                    pathEditor.OnUpdated += UpdateBlock;


                    while (selectedBlock == block)
                    {
                        pathEditor.lengthCap = timelineEditor.freeTimeInTimeline;
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
                        guardBlock.aimCone = new VisionCone(originPoint, blockOwnerStats.range, blockOwnerStats.spread);
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

            Debug.Log($"[BlockEditor] - Stopped editing block {block.id}");
        }
    }

    public void UpdateBlock()
    {
        if (selectedBlock == null)
        {
            return;
        }

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

                    moveBlock.duration = moveBlock.movementPath.GetTotalPathLength() / blockOwnerStats.speed;

                }
                break;
                case BlockType.Guard:
                {
                    OnBlockSet?.Invoke(block.id);
                }
                break;
            }

        }
        OnUpdated?.Invoke();
    }
}
