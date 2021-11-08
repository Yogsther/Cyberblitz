using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineVisual : MonoBehaviour
{
    public MoveBlockVisual moveBlockVisualPrefab;
   // public GuardBlockVisual guardBlockVisualPrefab;

    public UnitID ownerUnitId;

    public Timeline visualizedTimeline;

    List<BlockVisual> blockVisuals;

    public void UpdateBlockVisuals()
    {
        foreach(Block block in visualizedTimeline.blocks)
        {
            
        }
    }

    public bool TryGetBlockVisual(Block block, out BlockVisual output)
    {
        output = null;

        foreach(BlockVisual )
    }

}
