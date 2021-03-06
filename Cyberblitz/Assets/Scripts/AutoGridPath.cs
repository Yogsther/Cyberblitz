using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WayStarPathfinding;

public class GridPoint
{
	public Vector2Int point;

	public GridPoint(Vector2Int point)
	{
		this.point = point;
	}
}

public class AutoGridPath
{

	public GridPoint origin;
	public GridPoint target;

	public AutoGridPath()
	{

	}

	public AutoGridPath(GridPoint lastTarget)
	{
		this.origin = lastTarget;
		this.target = new GridPoint(origin.point);
	}

	//public List<Vector2Int> points => pathFinder.GetPath(origin.point, target.point, 10000);

	private PathFinder pathFinder = new PathFinder();

	public List<Section> sections => GetSections();

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

	public List<Vector2Int> GetPoints()
	{
		pathFinder.blockMask = LayerMask.GetMask("Wall");

		return pathFinder.GetPath(origin.point, target.point, 10000);
	}

	public List<Section> GetSections()
	{
		List<Section> pathSections = new List<Section>();

		List<Vector2Int> points = GetPoints();

		for (int i = 0; i < points.Count - 1; i++)
		{
			Vector2Int thisPoint = points[i];
			Vector2Int nextPoint = points[i + 1];

			Section section = new Section(thisPoint, nextPoint);

			pathSections.Add(section);
		}

		return pathSections;
	}

	public List<Section> GetSimplifiedSections()
    {
		List<Section> simplifiedSections = new List<Section>();
		List<Section> baseSections = GetSections();

		if (baseSections.Count != 0)
		{

			Vector2Int lastDirection = baseSections[0].Direction;

			Section simplifiedSection = baseSections[0];

			foreach (Section section in baseSections)
			{
				if (section.Direction != lastDirection)
				{
					simplifiedSection.b = section.a;

					simplifiedSections.Add(simplifiedSection);

					simplifiedSection = section;
				}

				lastDirection = section.Direction;
			}

			simplifiedSection.b = baseSections[baseSections.Count - 1].b;

			simplifiedSections.Add(simplifiedSection);

		}

		return simplifiedSections;

	}

	public List<Vector2Int> GetSimpifiedPoints()
	{
		List<Vector2Int> simplifiedPoints = new List<Vector2Int>();

		List<Section> simplifiedSections = GetSimplifiedSections();

		if (simplifiedSections.Count != 0)
		{

			simplifiedPoints.Add(simplifiedSections[0].a);

			foreach (Section section in simplifiedSections)
			{
				simplifiedPoints.Add(section.b);
			}
		}

		return simplifiedPoints;
	}

	public float GetTotalPathLength()
	{
		float length = 0f;

		List<Vector2Int> points = GetPoints();

		for(int i = 0; i < points.Count - 1; i++)
        {
			Vector2Int a = points[i];
			Vector2Int b = points[i + 1];

			length += Vector2Int.Distance(a, b);
        }

		return length;
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
			} else
			{
				float lerpValue = Mathf.InverseLerp(0f, section.Length, distanceToPoint);

				return section.Lerp(lerpValue);
			}
		}

		return target.point;
	}


	public void DrawGizmos()
	{
		pathFinder.DrawGizmos();

		foreach (Section section in sections)
		{
			Gizmos.DrawLine(section.a.ToFlatVector3(.1f), section.b.ToFlatVector3(.1f));
		}
	}
}