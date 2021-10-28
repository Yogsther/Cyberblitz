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

	public float GetTotalPathLength()
	{
		float sectionsLength = 0f;

		foreach (Section section in sections)
		{
			sectionsLength += section.Length;
		}

		return sectionsLength;
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