using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;


public class GridPath
{
    public List<Vector2Int> points { get; private set; }

    public List<Section> sections = new List<Section>();

    public Action OnUpdated;

    public GridPath()
    {
        points = new List<Vector2Int>();
        sections = new List<Section>();
    }

    public struct Section
    {
        public Vector2Int a;
        public Vector2Int b;

        public Section(Vector2Int a, Vector2Int b)
        {
            this.a = a;
            this.b = b;
        }

        public Vector2Int Direction => (b - a);
        public float Length => Vector2Int.Distance(a, b);

        public Vector2 Lerp(float t)
        {
            return Vector2.Lerp(a, b, t);
        }
    }

    public void UpdateSections()
    {
        List<Section> pathSections = new List<Section>();

        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector2Int thisPoint = points[i];
            Vector2Int nextPoint = points[i + 1];

            Section section = new Section(thisPoint, nextPoint);

            pathSections.Add(section);
        }

        sections = pathSections;
    }

    public float GetTotalPathLength()
    {
        float sectionsLength = 0f;

        foreach (Section section in sections)
        {
            sectionsLength += section.Length;
        }

        return sectionsLength;
    }


    public List<int> GetIndexesOfPointsWithinRange(Vector2 origin, float radius)
    {
        List<int> indexes = new List<int>();

        for (int i = 0; i < points.Count - 1; i++)
        {
            if (Vector2.Distance(origin, points[i]) < radius)
            {
                indexes.Add(i);
            }
        }

        return indexes;
    }



    /// <summary>
    /// Finds an interpolated point on the path using <paramref name="t"/> * <see cref="GetTotalPathLength"/>.
    /// </summary>
    public Vector2 Interpolate(float t)
    {

        float distanceToPoint = t * GetTotalPathLength();

        foreach (Section section in sections)
        {
            if (distanceToPoint > section.Length)
            {
                distanceToPoint -= section.Length;
            }
            else
            {
                float lerpValue = Mathf.InverseLerp(0f, section.Length, distanceToPoint);

                return section.Lerp(lerpValue);
            }
        }

        return points[points.Count - 1];
    }

    public void SetPoints(List<Vector2Int> points)
    {
        points.Clear();

        points.AddRange(points);

        UpdateSections();

        OnUpdated?.Invoke();
    }

    public void AddPoint(Vector2Int point)
    {

        points.Add(point);

        UpdateSections();

        OnUpdated?.Invoke();
    }

    public void RemovePointAtIndex(int index)
    {
        points.RemoveAt(index);

        UpdateSections();
        OnUpdated?.Invoke();
    }



    public void DrawGizmos()
    {
        foreach (Section section in sections)
        {
            Gizmos.DrawLine(section.a.ToFlatVector3(), section.b.ToFlatVector3());
        }
    }
}


