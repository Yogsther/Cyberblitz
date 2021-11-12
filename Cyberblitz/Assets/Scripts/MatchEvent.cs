using UnityEngine;

public enum MatchEventType
{

	Shoot,
	Death,
	Damage
}

public abstract class MatchEvent
{
	public MatchEventType type;
	public UnitID actorUnitId;
	public float time;

	public MatchEvent(UnitID actorUnitId, float time)
	{
		this.actorUnitId = actorUnitId;
		this.time = time;
	}

	public abstract void PlaybackEffect(Match simulatedMatch);
}



public class ShootEvent : MatchEvent
{
	public UnitID affectedUnitId;
	public bool isHit;
	public ShootEvent(UnitID actorUnitId, UnitID affectedUnitId, bool isHit, float time) : base(actorUnitId, time)
	{
		type = MatchEventType.Shoot;
		this.affectedUnitId = affectedUnitId;
		this.isHit = isHit;
	}

	public override void PlaybackEffect(Match simulatedMatch)
	{
		VisualUnit shooter = VisualUnitManager.GetVisualUnitById(actorUnitId);
		VisualUnit victim = VisualUnitManager.GetVisualUnitById(affectedUnitId);

		Vector3 shooterPos = shooter.mainModel.position;
		Vector3 victimPos = victim.mainModel.position;

		Vector3 fromShooterToVictim = (victimPos - shooterPos).Flatten();

		shooter.SetTargetForward(fromShooterToVictim);

		Unit shooterUnit = simulatedMatch.GetUnit(actorUnitId);
		UnitData shooterData = UnitDataManager.GetUnitDataByType(shooterUnit.type);

		AudioClip fireSound = shooterData.fireSounds[Random.Range(0, shooterData.fireSounds.Length)];

		if (isHit) SoundManager.PlaySound(fireSound, shooter.mainModel.transform.position);
		else SoundManager.PlaySound("missed_shot", shooter.mainModel.transform.position);

		SoundManager.PlaySound("shell_drop", shooter.mainModel.transform.position, 500f);

		shooter.animator.SetTrigger("FireTrigger");
	}
}

public class DamageEvent : MatchEvent
{
	public float damageAmount;

	public DamageEvent(UnitID actorUnitId, float damageAmount, float time) : base(actorUnitId, time)
	{
		type = MatchEventType.Damage;
		this.damageAmount = damageAmount;
	}

	public override void PlaybackEffect(Match simulatedMatch)
	{
		VisualUnit visualUnit = VisualUnitManager.GetVisualUnitById(actorUnitId);
		visualUnit.animator.SetTrigger("Hit");
	}
}

public class DeathEvent : MatchEvent
{
	public DeathEvent(UnitID actorUnitId, float time) : base(actorUnitId, time)
	{
		type = MatchEventType.Death;
	}

	public override void PlaybackEffect(Match simulatedMatch)
	{
		VisualUnit.OnDeath?.Invoke(actorUnitId);
	}
}
