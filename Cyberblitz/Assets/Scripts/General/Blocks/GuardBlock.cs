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

		DataManager.unitStats.TryGetUnitStatsByType(ownerUnit.type, out UnitStats ownerUnitStats);

		foreach (Unit otherUnit in simulatedMatch.GetAllUnits())
		{
			if (otherUnit.IsDead()) continue;

			int otherUnitTeam = simulatedMatch.GetUnitOwner(otherUnit.id).team;

			bool otherUnitIsWithinAimCone = aimCone.GetPointIsWithinCone(otherUnit.position.ToVector2());

			if (otherUnitTeam != ownerTeam && otherUnitIsWithinAimCone)
			{

				float shotCooldown = 1f / ownerUnitStats.firerate;
				float secondsSinceLastShot = localTime - ownerUnit.lastShot;
				bool canShoot = ownerUnit.lastShot == -1 || secondsSinceLastShot >= shotCooldown;


				if (canShoot && !otherUnit.dead)
				{


					float effectAnimationDelay = .2f;
					float effectTime = ownerUnit.timeline.GetStartTimeOfBlock(this) + localTime;

					float hitChance = 1f;

					if (DataManager.levelLayouts.TryGetLevelLayout(simulatedMatch.level, out LevelLayout levelLayout))
					{
						hitChance = CalculateHitChance(otherUnit, levelLayout);
					} else
					{
						Debug.Log("No level layout found, skipping hitChance calculation");
					}

					Debug.Log($"[GuardBlock] - HIT CHANCE: {hitChance}");

					bool isHit = hitChance > Random.value;

					if (hitChance > 0)
					{
						ShootEvent shootEvent = new ShootEvent(ownerUnit.id, otherUnit.id, isHit, effectTime);
						simulatedMatch.events.Enqueue(shootEvent);

						ownerUnit.lastShot = localTime;

						if (isHit)
						{
							otherUnit.hp -= ownerUnitStats.damage;
						}

						if (otherUnit.hp <= 0)
						{
							otherUnit.hp = 0;
							otherUnit.dead = true;

							DeathEvent deathEvent = new DeathEvent(otherUnit.id, effectTime + effectAnimationDelay);
							simulatedMatch.events.Enqueue(deathEvent);

							if (otherUnit.type == UnitType.Courier && simulatedMatch.winner == null)
							{
								simulatedMatch.winner = ownerUnit.ownerID;
							}
						} else
						{
							DamageEvent damageEvent = new DamageEvent(otherUnit.id, ownerUnitStats.damage, effectTime + effectAnimationDelay);
							simulatedMatch.events.Enqueue(damageEvent);
						}
					}

				}
			}
		}
	}

	private float CalculateHitChance(Unit targetUnit, LevelLayout levelLayout)
	{
		float hitChance = 1f;

		Vector2 originPosition = aimCone.origin.point;
		Vector2 targetPosition = targetUnit.position.ToVector2();

		Vector3 rayOrigin = originPosition.ToFlatVector3(.25f);
		Vector3 rayDirection = (targetPosition - originPosition).ToFlatVector3(.25f);

		Ray shootRay = new Ray(rayOrigin, rayDirection);

		float distanceToTarget = Vector2.Distance(originPosition, targetPosition);

		foreach (GridCollider gridCollider in levelLayout.gridColliders)
		{
			if (LineIntersectionTest.Intersect(originPosition, targetPosition, gridCollider.collisionRect))
			{
				Debug.Log(gridCollider.type);

				if (gridCollider.type == ColliderType.Full)
				{
					return 0f;
				}

				if (gridCollider.type == ColliderType.Half)
				{
					hitChance = .5f;
				}
			}
		}

		return hitChance;
	}

	public override void OnPlaybackStart(Match simulatedMatch)
	{
		VisualUnit ownerVisualUnit = VisualUnitManager.GetVisualUnitById(ownerId);
		ownerVisualUnit.animator.SetTrigger("Stop");

		Quaternion targetRotation = Quaternion.AngleAxis(aimCone.direction, Vector3.down);

		ownerVisualUnit.SetTargetForward(targetRotation * Vector3.forward);
	}

	public override void Playback(Match simulatedMatch, float localTime)
	{

	}

	public override void OnPlaybackEnd(Match simulatedMatch)
	{
	}
}