using UnityEngine;

public class VisionCone
{
	public Vector2Int origin;
	public float direction;
	public float radius;
	public float angleWidth;

	public Vector2[] GetConePoints()
	{
		Vector2[] points = new Vector2[3];
		Vector2[] directions = GetConeDirections();

		points[0] = origin;
		points[1] = origin + directions[0];
		points[2] = origin + directions[1];

		return points;
	}

	public Vector2[] GetConeDirections()
	{
		Vector2[] directions = new Vector2[2];

		float directionOffsetDegrees = angleWidth * .5f;

		Quaternion leftOffset = Quaternion.AngleAxis(direction - directionOffsetDegrees, Vector3.down);
		Quaternion rightOffset = Quaternion.AngleAxis(direction + directionOffsetDegrees, Vector3.down);

		directions[0] = (leftOffset * Vector3.right * radius).FlatVector3ToVector2();
		directions[1] = (rightOffset * Vector3.right * radius).FlatVector3ToVector2();

		return directions;
	}

	public bool GetPointIsWithinCone(Vector2 point)
	{
		float distance = Vector2.Distance(origin, point);

		if (distance > radius) return false;

		Vector2 pointDirection = point - origin;

		float pointAngle = Mathf.Atan2(pointDirection.y, pointDirection.x) * Mathf.Rad2Deg;


		if (Mathf.DeltaAngle(pointAngle, direction) > angleWidth * .5f) return false;
		if (Mathf.DeltaAngle(direction, pointAngle) > angleWidth * .5f) return false;


		return true;
	}


	public void DrawGizmo()
	{
		Vector2[] points = GetConePoints();

		Gizmos.DrawLine(points[0].ToFlatVector3(), points[1].ToFlatVector3());
		Gizmos.DrawLine(points[0].ToFlatVector3(), points[2].ToFlatVector3());
	}
}