using System.Collections.Generic;
using UnityEngine;

public class VisualGuardBlock : VisualBlock
{
    public GuardBlock guardBlock => block as GuardBlock;

    public LayerMask blockingLayerMask;

    [Header("Temp Cone")]
    public GameObject tempCone;
    public MeshFilter tempConeMeshFilter;
    public MeshRenderer tempConeMeshRenderer;

    private Mesh tempConeMesh;

    [Header("Locked Cone")]
    public GameObject lockedCone;

    public MeshFilter lockedConeMeshFilter;
    public MeshRenderer lockedConeMeshRenderer;

    private Mesh lockedConeMesh;

    [Header("Half Cover Mesh")]
    public GameObject coverCone;

    public MeshFilter coverConeMeshFilter;
    public MeshRenderer coverConeMeshRenderer;

    private Mesh coverConeMesh;


    public float smoothingSpeed = 20f;

    private Vector2[] drawnPoints;

    private void Start()
    {
        tempConeMesh = new Mesh();
        lockedConeMesh = new Mesh();
        coverConeMesh = new Mesh();

        tempConeMesh.name = "Temporary Cone";
        lockedConeMesh.name = "Locked Cone";
        coverConeMesh.name = "Cover Cone";

        tempConeMeshFilter.mesh = tempConeMesh;
        lockedConeMeshFilter.mesh = lockedConeMesh;
        coverConeMeshFilter.mesh = coverConeMesh;
    }

    public override void SetBlock(Block block)
    {
        base.SetBlock(block);


    }

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


        //tempConeMeshFilter.mesh = GetConeMesh(testTempPoints);
        //lockedConeMeshFilter.mesh = GetConeMesh(testLockPoints);

        UpdateConeMesh(ref tempConeMesh, testTempPoints);
        UpdateConeMesh(ref lockedConeMesh, testLockPoints);

    }

    public override void SetSelected(bool status)
    {
        base.SetSelected(status);

        UpdateVisuals();

    }

    private void Update()
    {
        if (guardBlock.aimCone != null)
        {

            tempCone.SetActive(selected && !InputManager.isOnGui);

            if (tempCone.activeInHierarchy)
            {
                float inputDirection = GameManager.instance.TimelineEditor.blockEditor.visionConeEditor.GetInputDirection();

                Vector2[] points = guardBlock.aimCone.GetConePoints(inputDirection);

                UpdateConeMesh(ref tempConeMesh, points);
            }

            if (lockedCone.activeInHierarchy)
            {

                Vector2[] points = guardBlock.aimCone.GetConePoints();

                if (drawnPoints != null)
                {
                    for (int i = 0; i < points.Length; i++)
                    {
                        drawnPoints[i] = Vector2.LerpUnclamped(drawnPoints[i], points[i], smoothingSpeed * Time.deltaTime);
                    }
                    UpdateConeMesh(ref lockedConeMesh, drawnPoints);
                }
                else
                {
                    UpdateConeMesh(ref lockedConeMesh, points);
                }

            }
        }
    }

    public override void UpdateVisuals()
    {
        if (guardBlock.aimCone != null && guardBlock.aimCone.isSet && !lockedCone.activeInHierarchy)
        {
            drawnPoints = new Vector2[3];
            drawnPoints.SetAllIndexesToValue(guardBlock.aimCone.GetConePoints()[0]);

            lockedCone.SetActive(true);
        }
    }

    private void UpdateConeMesh(ref Mesh mesh, Vector2[] conePoints)
    {
        mesh.Clear();
        coverConeMesh.Clear();

        GetConeVertices(ref mesh, conePoints);
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
    }

    private void GetConeVertices(ref Mesh mesh, Vector2[] points)
    {
        float lineHeight = .21f;

        Vector2 origin = points[0];

        float coneDistance = Vector2.Distance(origin, points[1]);

        int coneResolution = Mathf.CeilToInt(Vector2.Angle(points[1], points[2]) * coneDistance * .33f);



        Vector2 dirA = points[1] - origin;
        Vector2 dirB = points[2] - origin;

        bool isInverted = Vector2.SignedAngle(dirA, dirB) < 0f;

        Vector2 startDiration = isInverted ? dirA : dirB;
        Vector2 endDirection = isInverted ? dirB : dirA;
        float startAngle = Mathf.Atan2(startDiration.y, startDiration.x) * Mathf.Rad2Deg;
        float endAngle = Mathf.Atan2(endDirection.y, endDirection.x) * Mathf.Rad2Deg;



        //List<Vector3> verts = new List<Vector3>();
        List<Vector2> outerPoints = new List<Vector2>();

        List<Vector2> halfCoveredPoints = new List<Vector2>();

        ConeCastInfo oldConeCast = new ConeCastInfo();

        for (int i = 0; i < coneResolution; i++)
        {
            float lerpVal = i / (float)coneResolution;//Mathf.InverseLerp(0, coneResolution, i);

            float angle = Mathf.LerpAngle(startAngle, endAngle, lerpVal) * Mathf.Deg2Rad;
            float x = (Mathf.Cos(angle) * coneDistance) + origin.x;
            float y = (Mathf.Sin(angle) * coneDistance) + origin.y;

            Vector2 point = new Vector2(x, y);

            ConeCastInfo newConeCast = ConeCast(origin, point);

            if (i > 0)
            {
                if (oldConeCast.hit != newConeCast.hit)
                {

                    outerPoints.Add(newConeCast.point);

                   // Edge edge = CalculateBlocking(oldConeCast, newConeCast);

                    //outerPoints.Add(edge.min);
                    //outerPoints.Add(edge.max);

                }
            }

            outerPoints.Add(newConeCast.point);

            if (newConeCast.secondPoint != Vector2.zero)
            {
                ConeCastInfo ccInfo = ConeCast(newConeCast.point, newConeCast.secondPoint, true);

                halfCoveredPoints.Add(ccInfo.origin);
                halfCoveredPoints.Add(ccInfo.point);

            }

            oldConeCast = newConeCast;
        }


        int vertexCount = outerPoints.Count + 1;
        int triangleCount = (vertexCount - 2) * 3;

        Vector3[] verts = new Vector3[vertexCount];
        int[] tris = new int[triangleCount];

        verts[0] = origin.ToFlatVector3(lineHeight);


        for (int i = 0; i < vertexCount - 1; i++)
        {
            int v = i + 1;
            int t = i * 3;

            verts[v] = outerPoints[i].ToFlatVector3(lineHeight);

            if (i < vertexCount - 2)
            {
                tris[t + 0] = 0;
                tris[t + 1] = i + 1;
                tris[t + 2] = i + 2;

            }

        }



        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0);

        if (halfCoveredPoints.Count > 0)
        {

            int coverVertCount = halfCoveredPoints.Count;
            int coverTriCount = (halfCoveredPoints.Count - 2) * 3;

            Vector3[] coverVerts = new Vector3[coverVertCount];
            int[] halfCoverTris = new int[coverTriCount];

            for (int i = 0; i < coverVertCount; i++)
            {
                int v = i;
                int t = i * 3;

                coverVerts[v] = halfCoveredPoints[i].ToFlatVector3(lineHeight);

                if (i < coverVertCount - 2)
                {
                    if (i % 2 == 0)
                    {
                        halfCoverTris[t + 0] = i + 0;
                        halfCoverTris[t + 1] = i + 1;
                        halfCoverTris[t + 2] = i + 2;
                    }
                    else
                    {
                        halfCoverTris[t + 0] = i + 2;
                        halfCoverTris[t + 1] = i + 1;
                        halfCoverTris[t + 2] = i + 0;
                    }
                    
                }
            }

            coverConeMesh.SetVertices(coverVerts);
            coverConeMesh.SetTriangles(halfCoverTris, 0);

        }
        //verts.Insert(0, origin.ToFlatVector3(lineHeight));

        //return verts.ToArray();
    }

    public struct Edge
    {
        public Vector2 min;
        public Vector2 max;

        public bool isBlocked;
        public Vector2 Average => Vector3.Cross(min, max);
    }

    public struct ConeCastInfo
    {
        public bool hit;
        public Vector2 origin;
        public Vector2 point;
        public float distance;
        public float angle;

        public float secondDistance;
        public Vector2 secondPoint;
    }

    /*    public ConeCastInfo ConeCast(Vector2 origin, Vector2 point)
        {
            foreach (GridCollider gridCollider in LevelManager.instance.currentLevel.gridColliders)
            {
                LineIntersectionTest.LineIntersectsRect(origin, point, gridCollider.collisionRect, out Vector2 hitPoint);
            }
        }*/
    private ConeCastInfo ConeCast(Vector2 origin, Vector2 point, bool ignoreHalfColliders = false)
    {
        Vector2 originalPoint = point;
        float originalDistance = Vector2.Distance(origin, point);

        ConeCastInfo info = new ConeCastInfo
        {
            hit = false,
            origin = origin,
            point = point,
            distance = Vector2.Distance(origin, point),

            secondPoint = Vector2.zero
        };

        Vector2 dir = (point - origin).normalized;

        foreach (GridCollider gridCollider in LevelManager.instance.currentLevel.gridColliders)
        {

            if (LineIntersectionTest.LineIntersectsRect(info.origin, info.point, gridCollider.collisionRect, out Vector2 hitPoint))
            {
                if(gridCollider.type == ColliderType.Full)
                {
                    info.hit = true;
                    info.point = hitPoint;
                    info.distance = (info.point - origin).magnitude;
                }
                

                if (gridCollider.type == ColliderType.Half && !ignoreHalfColliders)
                {
                    info.hit = true;
                    info.point = hitPoint;
                    info.distance = (info.point - origin).magnitude;
                    info.secondDistance = (originalDistance - info.distance);
                    info.secondPoint = originalPoint;
                }
            }
        }

        return info;
    }

    private Edge CalculateBlocking(ConeCastInfo minConeCast, ConeCastInfo maxConeCast)
    {

        Edge edge = new Edge
        {
            min = minConeCast.point,
            max = maxConeCast.point
        };


        for (int i = 0; i < 10; i++)
        {

            ConeCastInfo newConeCast = ConeCast(minConeCast.origin, edge.Average);

            if (newConeCast.hit == minConeCast.hit)
            {
                edge.min = newConeCast.point;
            }
            else
            {
                edge.max = newConeCast.point;
            }

        }

        return edge;

        /*if (gridCollider.type == ColliderType.Full)
        {
            return 0f;
        }

        if (gridCollider.type == ColliderType.Half)
        {
            hitChance = .5f;
        }*/


    }

    private void OnDrawGizmos()
    {
        if (guardBlock != null)
        {
            guardBlock.aimCone.DrawGizmo();
        }
    }
}
