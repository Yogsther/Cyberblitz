using UnityEngine;

public class Unit
{
	public UnitID id = UnitID.New;
	public UserID ownerID;
	public float hp;
	public UnitType type;
	public bool isMVP = false;
	public Timeline timeline = new Timeline();
	public Position position = new Position(0f, 0f);
	public Position gridPosition;

	// The time they last shot, undefined value is -1
	// Used by Guard simulator to implement fire-rate.
	public float lastShot = -1;

	public Unit(UserID ownerID)
	{
		this.ownerID = ownerID;
		timeline.ownerId = id;
	}

	public bool IsDead()
	{
		return hp <= 0;
	}
	public void SetPosition(Vector2Int spawn)
	{
		position = new Position(spawn.x, spawn.y);
		//gridPosition = position.ToVector2Int;
	}
}
