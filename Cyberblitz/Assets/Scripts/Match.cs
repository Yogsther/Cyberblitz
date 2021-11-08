using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match
{
	public enum GameState { MapSelection, Planning, Playback, Starting, Ending, WaitingForUnits }

	public MatchID id = MatchID.New;
	public Player[] players;
	public Unit[] units;
	public int round;
	public string level;
	public Queue<MatchEvent> events = new Queue<MatchEvent>();
	public GameState state = GameState.Starting;
	public MatchRules rules = new MatchRules();
	public float longestTimelineDuration;
	public UserID winner = null;

	public bool IsOwnerOfUnit(UserID userId, UnitID unitId)
	{
		Unit unit = GetUnit(unitId);
		return userId == unit.ownerID;
	}

	public Player GetPlayer(UserID userId)
	{
		foreach (Player player in players)
			if (player.user.id == userId) return player;
		return null;
	}

	public int GetLocalTeam()
	{
		return GetPlayer(ClientLogin.user.id).team;
	}

	public User GetUser(UserID userId)
	{
		Player player = GetPlayer(userId);
		if (player != null) return player.user;
		return null;
	}

	// Should prob be rewritten..
	public Unit[] GetAllUnits()
	{
		int amount = 0;
		int index = 0;
		foreach (Player player in players) amount += player.units.Length;
		Unit[] units = new Unit[amount];
		foreach (Player player in players)
		{
			foreach (Unit unit in player.units)
			{
				units[index] = unit;
				index++;
			}
		}
		return units;
	}

	/// <summary>
	/// Get all units from a certain team
	/// </summary>
	/// <param name="team">The team you want to get units from</param>
	public Unit[] GetAllUnits(int team)
	{
		int amount = 0;
		int index = 0;
		foreach (Player player in players) if (player.team == team) amount += player.units.Length;
		Unit[] units = new Unit[amount];
		foreach (Player player in players)
		{
			if (player.team == team)
				foreach (Unit unit in player.units)
				{
					units[index] = unit;
					index++;
				}
		}
		return units;
	}

	public Unit GetUnit(UnitID id)
	{
		/*Debug.Log("Trying to get unit: " + id);*/
		foreach (Player player in players)
			foreach (Unit unit in player.units)
				if (unit.id == id) return unit;
		return null;
	}


	public Player GetUnitOwner(UnitID id)
	{
		foreach (Player player in players)
		{
			foreach (Unit unit in player.units)
			{
				if (unit.id == id) return player;
			}
		}

		return null;
	}

	public int GetUnitTeam(UnitID id)
	{
		return GetUnitOwner(id).team;
	}

	public bool ContainsPlayer(UserID id)
	{
		foreach (Player player in players)
			if (player.user.id == id) return true;
		return false;
	}


}