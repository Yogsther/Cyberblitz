using System.Collections.Generic;
using UnityEngine;

public class LineIntersectionTest : MonoBehaviour
{
    public LevelData levelData;

    public List<Rect> rects;

    public Vector2 pointA;
    public Vector2 pointB;
    public Vector2 pointC;
    public Vector2 pointD;

    public Vector2 offset;

    public List<Vector2> shape;


    public static bool Intersect(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 point)
    {
        point = a2;

        Vector2 aDir = (a2 - a1).normalized;
        Vector2 bDir = (b2 - b1).normalized;

        Vector2 aNormal = new Vector2(-aDir.y, aDir.x);
        Vector2 bNormal = new Vector2(-bDir.y, bDir.x);

        float aK = (aNormal.x * a1.x) + (aNormal.y * a1.y);
        float bK = (bNormal.x * b1.x) + (bNormal.y * b1.y);

        if (IsParallel(aNormal, bNormal))
        {
            return false;
        }

        if (IsOrthogonal(a1 - b1, aNormal))
        {
            return false;
        }

        float div = (aNormal.x * bNormal.y - aNormal.y * bNormal.x);

        float xIntersect = (bNormal.y * aK - aNormal.y * bK) / div;
        float yIntersect = (-bNormal.x * aK + aNormal.x * bK) / div;

        Vector2 intersectionPoint = new Vector2(xIntersect, yIntersect);

        if (IsBetween(a1, a2, intersectionPoint) && IsBetween(b1, b2, intersectionPoint))
        {
            point = intersectionPoint;
            return true;
        }

        return false;
    }

    public static bool IsParallel(Vector2 v1, Vector2 v2)
    {

        if (Vector2.Angle(v1, v2) == 0f || Vector2.Angle(v1, v2) == 180f)
        {
            return true;
        }

        return false;
    }

    public static bool IsOrthogonal(Vector2 v1, Vector2 v2)
    {
        //2 vectors are orthogonal is the dot product is 0
        //We have to check if close to 0 because of floating numbers
        if (Mathf.Abs(Vector2.Dot(v1, v2)) < 0.000001f)
        {
            return true;
        }

        return false;
    }

    //Is a point c between 2 other points a and b?
    public static bool IsBetween(Vector2 a, Vector2 b, Vector2 c)
    {
        bool isBetween = false;

        //Entire line segment
        Vector2 ab = b - a;
        //The intersection and the first point
        Vector2 ac = c - a;

        //Need to check 2 things: 
        //1. If the vectors are pointing in the same direction = if the dot product is positive
        //2. If the length of the vector between the intersection and the first point is smaller than the entire line
        if (Vector2.Dot(ab, ac) > 0f && ab.sqrMagnitude >= ac.sqrMagnitude)
        {
            isBetween = true;
        }

        return isBetween;
    }

    public struct Line
    {
        public Vector2 pointA;
        public Vector2 pointB;

        public Line(Vector2 a, Vector2 b)
        {
            this.pointA = a;
            this.pointB = b;
        }
    }

    public static bool LineIntersectsLine(Line lineA, Line lineB, out Vector2 intersectionPoint)
    {
        return Intersect(lineA.pointA, lineA.pointB, lineB.pointA, lineB.pointB, out intersectionPoint);
    }

    public static bool LineIntersectsRect(Vector2 a, Vector2 b, Rect rect)
    {
        return rect.Contains(a) 
            || rect.Contains(b)
            || Intersect(a, b, new Vector2(rect.x, rect.y), new Vector2(rect.xMax, rect.y), out Vector2 pointU)
            || Intersect(a, b, new Vector2(rect.x, rect.yMax), new Vector2(rect.xMax, rect.yMax), out Vector2 pointD)
            || Intersect(a, b, new Vector2(rect.x, rect.y), new Vector2(rect.x, rect.yMax), out Vector2 pointL)
            || Intersect(a, b, new Vector2(rect.xMax, rect.y), new Vector2(rect.xMax, rect.yMax), out Vector2 pointR);
    }

    public static bool LineIntersectsRect(Vector2 a, Vector2 b, Rect rect, out Vector2 intersectionPoint)
    {
        float closestIntersectionPoint = -1f;
        intersectionPoint = Vector2.zero;

        List<Line> rectLines = new List<Line>
        {
            new Line(new Vector2(rect.x, rect.y), new Vector2(rect.xMax, rect.y)),
            new Line(new Vector2(rect.xMax, rect.y), new Vector2(rect.xMax, rect.yMax)),
            new Line(new Vector2(rect.xMax, rect.yMax), new Vector2(rect.x, rect.yMax)),
            new Line(new Vector2(rect.x, rect.yMax), new Vector2(rect.x, rect.y))
        };

        List<Vector2> intersectionPoints = new List<Vector2>();

        foreach(Line line in rectLines)
        {
            if(Intersect(a, b, line.pointA, line.pointB, out Vector2 point))
            {
                intersectionPoints.Add(point);
            }
        }

        foreach(Vector2 point in intersectionPoints)
        {
            float distance = Vector2.Distance(a, point);

            if (distance < closestIntersectionPoint || closestIntersectionPoint == -1f)
            {
                closestIntersectionPoint = distance;
                intersectionPoint = point;
            }
        }

        bool isIntersecting = LineIntersectsRect(a,b,rect);
        return isIntersecting;
    }

    public static bool Intersect(Vector2 a, Vector2 b, List<Vector2> rectPoints, out Vector2 point)
    {
        point = Vector2.zero;
        for(int i = 0; i < rectPoints.Count; i++)
        {
            Vector2 thisPoint = rectPoints[i];
            Vector2 nextPoint = rectPoints[(i + 1) % rectPoints.Count];

            if (Intersect(a, b, thisPoint, nextPoint, out point))
            {
                return true;
            }
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(levelData.levelPrefab.levelGridSize.ToFlatVector3() * .5f, levelData.levelPrefab.levelGridSize.ToFlatVector3());

        foreach (GridCollider gc in levelData.levelPrefab.gridColliders)
        {
            Rect rect = gc.collisionRect;

            if (LineIntersectsRect(pointA + offset, pointB + offset, gc.collisionRect))
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.red;
            }

            

            Gizmos.DrawWireCube(gc.center, rect.size);

        }


        if (Intersect(pointA + offset, pointB + offset, shape, out Vector2 point))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(point, .1f);
        }
        else
        {
            Gizmos.color = Color.red;
        }

        for (int i = 0; i < shape.Count; i++)
        {
            Vector2 thisPoint = shape[i];
            Vector2 nextPoint = shape[(i + 1) % shape.Count];

            Gizmos.DrawLine(thisPoint, nextPoint);
        }

        Gizmos.DrawLine((pointA + offset), (pointB + offset));

    }
}