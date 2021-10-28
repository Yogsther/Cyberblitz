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

	public Unit(UserID ownerID)
	{
		this.ownerID = ownerID;
		timeline.ownerId = id;
	}

	public void SetPosition(int x, int y)
	{
		position = new Position(x, y);
		//gridPosition = position.ToVector2Int;
	}
}
