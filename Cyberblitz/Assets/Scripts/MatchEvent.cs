using System;
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

	public bool hasDonePre;
	public bool hasDonePost;

	public MatchEvent(UnitID actorUnitId, float time)
	{
		this.actorUnitId = actorUnitId;
		this.time = time;
	}

	public virtual void PreEffect()
    {
		this.LogWithOriginTag("Pre");

		hasDonePre = true;
	}

	public virtual void PostEffect()
	{
		this.LogWithOriginTag("Post");

		hasDonePost = true;
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

    public override void PreEffect()
    {
        base.PreEffect();

		VisualUnit shooter = VisualUnitManager.GetVisualUnitById(actorUnitId);
		VisualUnit victim = VisualUnitManager.GetVisualUnitById(affectedUnitId);

		Vector3 shooterPos = shooter.modelTransform.position;
		Vector3 victimPos = victim.modelTransform.position;

		Vector3 fromShooterToVictim = (victimPos - shooterPos).Flatten();

		shooter.SetTargetForward(fromShooterToVictim);

		shooter.isAiming = true;
	}

    public override void PlaybackEffect(Match simulatedMatch)
	{
        VisualUnit shooter = VisualUnitManager.GetVisualUnitById(actorUnitId);
        VisualUnit victim = VisualUnitManager.GetVisualUnitById(affectedUnitId);

        Vector3 shooterPos = shooter.modelTransform.position;
        Vector3 victimPos = victim.modelTransform.position;

        Vector3 fromShooterToVictim = (victimPos - shooterPos).Flatten();

        shooter.SetTargetForward(fromShooterToVictim);

        VisualUnit.OnShoot?.Invoke(actorUnitId, isHit);

		/*if (isHit) SoundManager.PlaySound(shooter.data.GetRandomFireSound(), shooterPos);
		else SoundManager.PlaySound("missed_shot", shooterPos);

		SoundManager.PlaySound("shell_drop", shooterPos, 500f);

		shooter.animator.SetTrigger("FireTrigger");*/
	}
}

public class DamageEvent : MatchEvent
{
	public float damageAmount;
	public static Action<UnitID, float> OnUnitDamage;

	public DamageEvent(UnitID actorUnitId, float damageAmount, float time) : base(actorUnitId, time)
	{
		type = MatchEventType.Damage;
		this.damageAmount = damageAmount;
	}

	public override void PlaybackEffect(Match simulatedMatch)
	{
		VisualUnit visualUnit = VisualUnitManager.GetVisualUnitById(actorUnitId);
		visualUnit.animator.SetTrigger("Hit");
		Debug.Log("Damge event!");
		OnUnitDamage?.Invoke(actorUnitId, damageAmount);
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
