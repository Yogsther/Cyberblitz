using System.Collections.Generic;
using UnityEngine;

public class VisualGuardBlock : VisualBlock
{
    public GuardBlock guardBlock => block as GuardBlock;

    [Header("Temp Cone")]
    public MeshFilter tempConeMeshFilter;
    public MeshRenderer tempConeMeshRenderer;

    [Header("Locked Cone")]
    public MeshFilter lockedConeMeshFilter;
    public MeshRenderer lockedConeMeshRenderer;

    public float smoothingSpeed = 20f;

    private Vector2[] drawnPoints;

    [ContextMenu("Draw Test Cones")]
    public void DrawTestCones()
    {
        Vector2[] testTempPoints = new Vector2[] {
            Vector2.zero,
            Quaternion.AngleAxis(-135f, Vector3.forward) * (Vector2.up * 5f),
            Quaternion.AngleAxis(135f, Vector3.forward) * (Vector2.up * 5f)
        };

        Vector2[] testLockPoints = new Vector2[] {
            Vector2.zero,
            Quaternion.AngleAxis(-45f, Vector3.forward) * (Vector2.up * 5f),
            Quaternion.AngleAxis(45f, Vector3.forward) * (Vector2.up * 5f)
        };


        tempConeMeshFilter.mesh = GetConeMesh(testTempPoints);
        lockedConeMeshFilter.mesh = GetConeMesh(testLockPoints);


    }

    public override void SetSelected(bool status)
    {
        base.SetSelected(status);

        UpdateVisuals();
        
    }

    private void Update()
    {
        tempConeMeshRenderer.gameObject.SetActive(selected && !InputManager.isOnGui);

        if (guardBlock != null && tempConeMeshRenderer.gameObject.activeInHierarchy)
        {

            float inputDirection = 0;
            if (!InputManager.isOnGui && InputManager.TryGetPointerHitLayer(LayerMask.GetMask("Ground"), out RaycastHit groundHit))
            {

                Vector2 mouseHitPoint = groundHit.point.FlatVector3ToVector2();

                Vector2 toMouse = mouseHitPoint - guardBlock.aimCone.origin.point;

                inputDirection = (Mathf.Atan2(toMouse.y, toMouse.x) * Mathf.Rad2Deg) - 90f;

                Vector2[] points = guardBlock.aimCone.GetConePoints(inputDirection);

                tempConeMeshFilter.mesh = GetConeMesh(points);
            }
        }

        if (guardBlock != null)
        {
            if (guardBlock.aimCone != null)
            {
                if (drawnPoints == null)
                {
                    drawnPoints = new Vector2[3];
                    drawnPoints.SetAllIndexesToValue(guardBlock.aimCone.origin.point);
                }

                Vector2[] points = guardBlock.aimCone.GetConePoints();

                for (int i = 0; i < points.Length; i++)
                {
                    drawnPoints[i] = Vector2.LerpUnclamped(drawnPoints[i], points[i], smoothingSpeed * Time.deltaTime);
                }


                    
                lockedConeMeshFilter.mesh = GetConeMesh(drawnPoints);

            }
        }

    }

    public override void UpdateVisuals()
    {

    }

    private Mesh GetConeMesh(Vector2[] conePoints)
    {
        Mesh mesh = new Mesh();

        Vector3[] verts = GetConeVertices(conePoints);
        int[] tris = GetConeTriangles(verts);

        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        mesh.Optimize();

        return mesh;
    }

    private Vector3[] GetConeVertices(Vector2[] points)
    {
        float lineHeight = .21f;

        Vector2 origin = points[0];

        float coneDistance = Vector2.Distance(origin, points[1]);

        int coneResolution = Mathf.CeilToInt(Vector2.Angle(points[1], points[2]) * coneDistance * .25f);

        List<Vector3> verts = new List<Vector3>();

        Vector2 dirA = points[1] - origin;
        Vector2 dirB = points[2] - origin;

        bool isInverted = Vector2.SignedAngle(dirA, dirB) < 0f;

        Vector2 startDiration = isInverted ? dirA : dirB;
        Vector2 endDirection = isInverted ? dirB : dirA;
        float startAngle = Mathf.Atan2(startDiration.y, startDiration.x) * Mathf.Rad2Deg;
        float endAngle = Mathf.Atan2(endDirection.y, endDirection.x) * Mathf.Rad2Deg;

        

        for (int i = 0; i < coneResolution + 1; i++)
        {
            float lerpVal = i / (float)coneResolution;//Mathf.InverseLerp(0, coneResolution, i);

            float angle = Mathf.LerpAngle(startAngle, endAngle, lerpVal) * Mathf.Deg2Rad;
            float x = (Mathf.Cos(angle) * coneDistance) + origin.x;
            float y = (Mathf.Sin(angle) * coneDistance) + origin.y;
            verts.Add(new Vector2(x, y).ToFlatVector3(lineHeight));
        }

        verts.Insert(0, origin.ToFlatVector3(lineHeight));

        return verts.ToArray();
    }

    private int[] GetConeTriangles(Vector3[] verts)
    {
        int[] tris = new int[(verts.Length + 1) * 3];

        for (int i = 0, t = 0; i < verts.Length - 2; i++, t += 3)
        {
            tris[t + 0] = 0;
            tris[t + 1] = i + 1;
            tris[t + 2] = i + 2;
        }

        return tris;
    }

    private List<Vector3> GetConeNormals(VisionCone cone)
    {
        int coneResolution = 16;
        List<Vector3> normals = new List<Vector3>();

        for (int i = 0; i < coneResolution; i++)
        {
            normals.Add(Vector3.up);
        }

        return normals;
    }

    private void OnDrawGizmos()
    {
        if(guardBlock != null)
        {
            guardBlock.aimCone.DrawGizmo();
        }
    }
}
