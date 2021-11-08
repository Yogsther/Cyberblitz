using System.Collections.Generic;
using UnityEngine;

public class MoveBlockVisual : BlockVisual
{
    public MoveBlock moveBlock => block as MoveBlock;

    public Waypoint waypoint;
    public LineRenderer moveLine;

    public bool selected;

    private List<Vector3> targetLinePositions = new List<Vector3>();

    private void Update()
    {
        DrawPath();
    }

    public override void UpdateVisuals()
    {
        UpdateTargetLinePositions();
    }

    private void UpdateTargetLinePositions()
    {
        float lineHeight = 0.21f;
        if (moveBlock.movementPath != null)
        {
            List<Vector2Int> points = moveBlock.movementPath.GetPoints();

            targetLinePositions = points.ToFlatVector3(lineHeight);

            int newPositions = targetLinePositions.Count - moveLine.positionCount;
            int newPositionsStartIndex = targetLinePositions.Count - newPositions - 1;

            moveLine.positionCount = targetLinePositions.Count;

            for (int i = moveLine.positionCount - 1; i > newPositionsStartIndex; i--)
            {
                moveLine.SetPosition(i, targetLinePositions[newPositionsStartIndex]);
            }

            // If no movementblocks were added, remove the line.
            if (moveLine.positionCount == 1)
            {
                moveLine.positionCount = 0;
            }
        }
    }

    private void DrawPath()
    {
        if (moveBlock.movementPath != null)
        {

            for (int i = 0; i < moveLine.positionCount; i++)
            {
                moveLine.SetPosition(i, Vector3.LerpUnclamped(moveLine.GetPosition(i), targetLinePositions[i], 10f * Time.deltaTime));
            }

            if (targetLinePositions.Count != 0)
            {
                waypoint.transform.position = moveLine.GetPosition(moveLine.positionCount - 1); //Vector3.LerpUnclamped(waypoint.transform.position, , 10f * Time.deltaTime);
            }
        }


    }


}
