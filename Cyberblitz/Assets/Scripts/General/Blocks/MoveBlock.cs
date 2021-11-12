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

	public override void OnPlaybackStart(Match simulatedMatch)
	{

	}

	public override void Playback(Match simulatedMatch, float localTime)
	{
		VisualUnit ownerVisualUnit = VisualUnitManager.GetVisualUnitById(ownerId);
		ownerVisualUnit.animator.SetTrigger("Run");

		Transform ownerTransform = ownerVisualUnit.mainModel;

		Vector3 newPosition = GetPositionAtTime(localTime).ToFlatVector3();

		Vector3 newForwardDirection = newPosition - ownerTransform.position;

		Quaternion newRotation = (newForwardDirection.magnitude != 0f) ? Quaternion.LookRotation(newForwardDirection, Vector3.up) : ownerTransform.rotation;

		ownerTransform.position = newPosition;

		ownerVisualUnit.SetTargetForward((newForwardDirection.magnitude != 0f) ? newForwardDirection : ownerTransform.forward);

		if (localTime + .1f > duration) ownerVisualUnit.animator.SetTrigger("Stop");
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

	public override void OnPlaybackEnd(Match simulatedMatch)
	{
		/*VisualUnit ownerVisualUnit = VisualUnitManager.GetVisualUnitById(ownerId);
		ownerVisualUnit.animator.SetTrigger("Stop");*/
	}
}