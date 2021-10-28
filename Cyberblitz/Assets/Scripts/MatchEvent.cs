﻿public enum MatchEventType
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
    public UnitID affectedUnit;
    public ShootEvent(UnitID actorUnitId, UnitID affectedUnit, float time) : base(actorUnitId, time)
    {
        type = MatchEventType.Shoot;

    }

    public override void PlaybackEffect(Match simulatedMatch)
    {
        throw new System.NotImplementedException();
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
        throw new System.NotImplementedException();
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
        VisualUnit visualUnit = VisualUnitManager.GetVisualUnitById(actorUnitId);

        visualUnit.gameObject.SetActive(false);
    }
}