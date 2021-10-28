using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardBlock : Block
{
	public VisionCone aimCone = new VisionCone();

	public GuardBlock(UnitID ownerId, int timelineIndex) : base(ownerId, timelineIndex)
	{
		type = BlockType.Guard;
	}
	public override void Simulate(Match simulatedMatch, float localTime)
	{
		int ownerTeam = simulatedMatch.GetUnitTeam(ownerId);

		foreach (Unit otherUnit in simulatedMatch.GetAllUnits())
		{
			int otherUnitTeam = simulatedMatch.GetUnitOwner(otherUnit.id).team;

			bool otherUnitIsWithinAimCone = aimCone.GetPointIsWithinCone(otherUnit.position.ToVector2());

			if (otherUnitTeam != ownerTeam && otherUnitIsWithinAimCone)
			{

				Debug.Log($"[GuardBlock - {ownerId}] Shot {otherUnit.id}");

			}
		}

	}

	public override void Playback(Match simulatedMatch, float localTime)
	{
		VisualUnit ownerVisualUnit = VisualUnitManager.GetVisualUnitById(ownerId);

		ownerVisualUnit.transform.rotation = Quaternion.AngleAxis(aimCone.direction, Vector3.up);
	}

}