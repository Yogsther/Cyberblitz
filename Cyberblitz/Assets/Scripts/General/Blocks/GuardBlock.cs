using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardBlock : Block
{
	public VisionCone aimCone;

	public GuardBlock(UnitID ownerId, int timelineIndex) : base(ownerId, timelineIndex)
	{
		type = BlockType.Guard;
	}
	public override void Simulate(Match simulatedMatch, float localTime)
	{

		Unit ownerUnit = simulatedMatch.GetUnit(ownerId);

		int ownerTeam = simulatedMatch.GetUnitTeam(ownerId);

		UnitStats ownerUnitStats = UnitDataManager.GetUnitDataByType(ownerUnit.type).stats;

		foreach (Unit otherUnit in simulatedMatch.GetAllUnits())
		{

			int otherUnitTeam = simulatedMatch.GetUnitOwner(otherUnit.id).team;

			bool otherUnitIsWithinAimCone = aimCone.GetPointIsWithinCone(otherUnit.position.ToVector2());

			if (otherUnitTeam != ownerTeam && otherUnitIsWithinAimCone)
			{
				if (otherUnit.IsDead()) continue;
				float shotsPerSecond = 1f / ownerUnitStats.firerate;
				float secondsSinceLastShot = localTime - ownerUnit.lastShot;
				bool canShoot = ownerUnit.lastShot == -1 || secondsSinceLastShot >= shotsPerSecond;
				Debug.Log("Can shoot: " + canShoot + " - last Shot " + ownerUnit.lastShot + ", localTime " + localTime);

				if (canShoot && !otherUnit.IsDead())
				{
					Debug.Log($"Shot");

					ShootEvent shootEvent = new ShootEvent(ownerUnit.id, otherUnit.id, localTime);
					simulatedMatch.events.Enqueue(shootEvent);

					ownerUnit.lastShot = localTime;
					otherUnit.hp -= ownerUnitStats.damage;

					if (otherUnit.hp <= 0)
					{
						Debug.Log("UNIT DIED");
						otherUnit.hp = 0;
						float deathTime = ownerUnit.timeline.GetStartTimeOfBlock(this) + localTime;
						DeathEvent deathEvent = new DeathEvent(otherUnit.id, deathTime);
						simulatedMatch.events.Enqueue(deathEvent);
					}

				}
			}


		}

	}

	public override void Playback(Match simulatedMatch, float localTime)
	{
		VisualUnit ownerVisualUnit = VisualUnitManager.GetVisualUnitById(ownerId);
		ownerVisualUnit.animator.SetTrigger("Stop");


		Transform ownerTransform = ownerVisualUnit.mainModel;

		ownerTransform.rotation = Quaternion.AngleAxis(aimCone.direction, Vector3.up);
	}

}