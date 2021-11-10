using System.Collections.Generic;
using UnityEngine;

public class VisualMoveBlock : VisualBlock
{

    public MoveBlock moveBlock => block as MoveBlock;

    public Waypoint waypoint;
    public LineRenderer moveLine;

    public float lineHeight = 0.21f;
    public float smoothingSpeed = 20f;



    private readonly List<Vector3> targetLinePositions = new List<Vector3>();

    public void Init()
    {
        waypoint.block = block;
        waypoint.SetSelected(selected);

        UpdatePositionsInstant();
    }

    private void Update()
    {
        DrawPath();

        
    }

    public override void SetSelected(bool status)
    {
        base.SetSelected(status);
        waypoint.SetSelected(status);
    }

    public override void UpdateVisuals()
    {
        UpdateTargetLinePositions();
        waypoint.SetSelected(selected);
    }

    private void UpdateTargetLinePositions()
    {

        if (moveBlock.movementPath != null)
        {
            targetLinePositions.Clear();

           /* float lerpMax = moveBlock.movementPath.GetTotalPathLength() * 3f;*/

            targetLinePositions.AddRange(moveBlock.movementPath.GetPoints().ToFlatVector3(lineHeight));


            /*for (float l = .2f; l <= lerpMax; l++)
            {
                float lerpValue = Mathf.InverseLerp(0f, lerpMax, l);

                Vector3 target = moveBlock.movementPath.Interpolate(lerpValue).ToFlatVector3(lineHeight);

                targetLinePositions.Add(target);
            }*/

            if (targetLinePositions.Count > 0)
            {

                for (int i = moveLine.positionCount; i < targetLinePositions.Count; i++)
                {
                    moveLine.positionCount++;

                    moveLine.SetPosition(i, waypoint.transform.position);
                }

            }
            
                moveLine.positionCount = Mathf.Min(moveLine.positionCount, targetLinePositions.Count);

            // If no movementblocks were added, remove the line.
            if (moveLine.positionCount == 1)
            {
                moveLine.positionCount = 0;
            }
        }
    }

    private void DrawPath()
    {
        if (moveLine.positionCount != 0)
        {
            moveLine.SetPosition(0, moveBlock.movementPath.origin.point.ToFlatVector3(lineHeight));

            for (int i = 1; i < moveLine.positionCount - 1; i++)
            {
                Vector3 currentPos = moveLine.GetPosition(i);
                Vector3 targetPos = targetLinePositions[i];

                moveLine.SetPosition(i, Vector3.LerpUnclamped(currentPos, targetPos, smoothingSpeed * Time.deltaTime));

                /*Debug.DrawRay(targetPos, Vector3.up, Color.green);

                Debug.DrawRay(moveLine.GetPosition(i), Vector3.up);*/
            }

            if (targetLinePositions.Count != 0)
            {
                Vector3 currentPos = waypoint.transform.position;
                Vector3 targetPos = targetLinePositions[targetLinePositions.Count - 1];

                waypoint.transform.position = Vector3.LerpUnclamped(currentPos, targetPos, smoothingSpeed * Time.deltaTime);


                moveLine.SetPosition(moveLine.positionCount - 1, waypoint.transform.position);

            }
        }


    }

    private void UpdatePositionsInstant()
    {
        if (moveLine.positionCount != 0)
        {
            for (int i = 0; i < moveLine.positionCount - 1; i++)
            {
                Vector3 targetPos = targetLinePositions[i];

                moveLine.SetPosition(i, targetPos);
            }

            if (targetLinePositions.Count != 0)
            {
                Vector3 targetPos = targetLinePositions[targetLinePositions.Count - 1];

                waypoint.transform.position = targetPos;
                moveLine.SetPosition(moveLine.positionCount - 1, waypoint.transform.position);

            }
        }

    }


}
