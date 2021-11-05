using System.Collections.Generic;

public class Player
{
	public Player(User user, int team)
	{
		this.user = user;
		this.team = team;
	}
	public User user;
	public Unit[] units = new Unit[4];
	public int team;
}