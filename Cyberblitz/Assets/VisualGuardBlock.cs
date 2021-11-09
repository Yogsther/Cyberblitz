using UnityEngine;

public class VisualGuardBlock : VisualBlock
{
    public GuardBlock guardBlock => block as GuardBlock;

    public LineRenderer temporaryCone;
    public LineRenderer lockedCone;


    private void Update()
    {
        if (guardBlock != null)
        {

            float inputDirection = 0;
            if (!InputManager.isOnGui && InputManager.TryGetPointerHitLayer(LayerMask.GetMask("Ground"), out RaycastHit groundHit))
            {

                Vector2 mouseHitPoint = groundHit.point.FlatVector3ToVector2();

                Vector2 toMouse = mouseHitPoint - guardBlock.aimCone.origin.point;

                inputDirection = (Mathf.Atan2(toMouse.y, toMouse.x) * Mathf.Rad2Deg) - 90f;

                Vector2[] points = guardBlock.aimCone.GetConePoints(inputDirection);
                DrawCone(points, temporaryCone);
            }
            else
            {
                temporaryCone.positionCount = 0;
            }

        }

    }

    public override void UpdateVisuals()
    {
        if (guardBlock.aimCone != null)
        {
            DrawCone(guardBlock.aimCone.GetConePoints(), lockedCone);
        }
    }


    private void DrawCone(Vector2[] points, LineRenderer line)
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

        float coneDistance = Vector2.Distance(points[0], points[1]);

        // Angle step is the resolution of the curved part of the cone
        float angleStep = (endAngle - startAngle) / coneResolution;

        for (int i = 0; i < coneResolution; i++)
        {
            float angle = Mathf.LerpAngle(startAngle * Mathf.Rad2Deg, endAngle * Mathf.Rad2Deg, i / (float)coneResolution) * Mathf.Deg2Rad;
            float x = (Mathf.Cos(angle) * coneDistance) + points[0].x;
            float y = (Mathf.Sin(angle) * coneDistance) + points[0].y;
            line.SetPosition(i + 2, new Vector2(x, y).ToFlatVector3(height));
        }

        line.SetPosition(line.positionCount - 2, points[2].ToFlatVector3(height));
        line.SetPosition(line.positionCount - 1, points[0].ToFlatVector3(height));
    }
}
