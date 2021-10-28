using UnityEngine;

public class MoveBlock : Block
{
	public AutoGridPath movementPath;
	//public new float duration => movementPath.GetTotalPathLength();

	public MoveBlock(UnitID ownerId, int timelineIndex) : base(ownerId, timelineIndex)
	{

		type = BlockType.Move;

		//movementPath = new AutoGridPath(MatchManager.match.GetUnit(ownerId).timeline.GetOriginPoint(timelineIndex));
	}

	public override void Simulate(Match simulatedMatch, float localTime)
	{
		Unit ownerUnit = simulatedMatch.GetUnit(ownerId);

		float sampleTime = Mathf.InverseLerp(0f, duration, localTime);

		ownerUnit.position = GetPositionAtTime(localTime).ToPosition();
	}

	public override void Playback(Match simulatedMatch, float localTime)
	{
		VisualUnit ownerVisualUnit = VisualUnitManager.GetVisualUnitById(ownerId);

		Transform ownerTransform = ownerVisualUnit.mainModel;

		Vector2 newPosition = GetPositionAtTime(localTime);

		Debug.Log(newPosition);

		Vector3 newForwardDirection = (GetPositionAtTime(localTime + 1f) - newPosition).ToFlatVector3();


		Quaternion newRotation = (newForwardDirection.magnitude != 0f) ? Quaternion.LookRotation(newForwardDirection, Vector3.up) : ownerTransform.rotation;

		ownerTransform.SetPositionAndRotation(newPosition.ToFlatVector3(), newRotation);
	}

	public Vector2 GetPositionAtTime(float localTime)
	{
		float sampleTime = Mathf.InverseLerp(0f, duration, localTime);

		return movementPath.Interpolate(sampleTime);
	}

	public Vector2 GetEndPosition()
	{
		return GetPositionAtTime(duration);
	}

}