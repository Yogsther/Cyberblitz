using System.Collections.Generic;
using UnityEngine;

public class MoveBlockVisual : BlockVisual
{
    public MoveBlock visualizedMoveBlock;

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
        if (visualizedMoveBlock.movementPath != null)
        {
            List<Vector2Int> points = visualizedMoveBlock.movementPath.GetPoints();

            targetLinePositions = points.ToFlatVector3(lineHeight);
        }
    }

    private void DrawPath()
    {
        if (visualizedMoveBlock.movementPath != null)
        {

            int newPositions = targetLinePositions.Count - moveLine.positionCount;
            int newPositionsStartIndex = targetLinePositions.Count - newPositions - 1;

            moveLine.positionCount = targetLinePositions.Count;

            for (int i = targetLinePositions.Count - 1; i > newPositionsStartIndex; i--)
            {
                moveLine.SetPosition(i, targetLinePositions[targetLinePositions.Count - 1]);
            }

            for (int i = 0; i < targetLinePositions.Count; i++)
            {

                moveLine.SetPosition(i, Vector3.LerpUnclamped(moveLine.GetPosition(i), targetLinePositions[i], 10f * Time.deltaTime));
            }
        }

        // If no movementblocks were added, remove the line.
        if (moveLine.positionCount == 1)
        {
            moveLine.positionCount = 0;
        }
    }

    
}
