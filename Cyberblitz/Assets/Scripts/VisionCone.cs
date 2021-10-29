using UnityEngine;

public class VisionCone
{
	public GridPoint origin;
	public float direction;
	public float radius;
	public float angleWidth;
	public bool isSet = false;

	public VisionCone()
	{
		direction = 0f;
	}

	public VisionCone(GridPoint origin, float radius, float angleWidth)
	{
		this.origin = origin;
		this.radius = radius;
		this.angleWidth = angleWidth;

		direction = 0f;
	}

	public Vector2[] GetConePoints()
	{
		return GetConePoints(direction);
	}

	public Vector2[] GetConePoints(float simulatedDirection)
	{
		Vector2[] points = new Vector2[3];
		Vector2[] directions = GetConeDirections(simulatedDirection);

		points[0] = origin.point;
		points[1] = origin.point + directions[0];
		points[2] = origin.point + directions[1];

		return points;
	}

	public Vector2[] GetConeDirections(float simulatedDirection)
	{
		Vector2[] directions = new Vector2[2];

		float directionOffsetDegrees = angleWidth * .5f;

		Quaternion leftOffset = Quaternion.AngleAxis(simulatedDirection - directionOffsetDegrees, Vector3.down);
		Quaternion rightOffset = Quaternion.AngleAxis(simulatedDirection + directionOffsetDegrees, Vector3.down);

		directions[0] = (leftOffset * Vector3.right * radius).FlatVector3ToVector2();
		directions[1] = (rightOffset * Vector3.right * radius).FlatVector3ToVector2();

		return directions;
	}

	public bool GetPointIsWithinCone(Vector2 point)
	{
		float distance = Vector2.Distance(origin.point, point);

		if (distance > radius) return false;

		Vector2 pointDirection = point - origin.point;

		float pointAngle = Mathf.Atan2(pointDirection.y, pointDirection.x) * Mathf.Rad2Deg;


		if (Mathf.DeltaAngle(pointAngle, direction) > angleWidth * .5f) return false;
		if (Mathf.DeltaAngle(direction, pointAngle) > angleWidth * .5f) return false;


		return true;
	}


	public void DrawGizmo()
	{
		Vector2[] points = GetConePoints();

		Gizmos.DrawLine(points[0].ToFlatVector3(.01f), points[1].ToFlatVector3(.01f));
		Gizmos.DrawLine(points[0].ToFlatVector3(.01f), points[2].ToFlatVector3(.01f));
	}
}