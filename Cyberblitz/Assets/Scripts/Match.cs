using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match
{
	public enum GameState { Planning, Playback, Starting, Ending, WaitingForUnits }

	public MatchID id = MatchID.New;
	public Player[] players;
	public Unit[] units;
	public int round;
	public string level;
	public Queue<MatchEvent> events = new Queue<MatchEvent>();
	public GameState state = GameState.Starting;
	public MatchRules rules = new MatchRules();
	public float longestTimelineDuration;

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

	public static Match GetSampleMatch()
	{
		Match sampleMatch = new Match();

		sampleMatch.id = MatchID.New;
		sampleMatch.players = new Player[]
		{
			new Player(new User(), 0),
			new Player(new User(), 1)
		};
		foreach (Player player in sampleMatch.players)
		{
			player.units = new Unit[]
			{
				new Unit(player.user.id),
				new Unit(player.user.id)
			};

			foreach (Unit unit in player.units)
			{
				UnitData unitData = UnitDataManager.GetUnitDataByType(UnitType.Scout);
				unit.hp = unitData.stats.maxHp;
				unit.type = unitData.type;
				unit.timeline.ownerId = unit.id;

				unit.position = new Position(2f, 2f);

				for (int i = 0; i < 2; i++)
				{
					MoveBlock moveBlock = new MoveBlock(unit.id, unit.timeline.GetSize());

					Vector2Int startPos = unit.timeline.GetEndPosition(unit.position.ToVector2()).RoundToVector2Int();


					unit.timeline.InsertBlock(moveBlock, i);

					//Debug.Log(moveBlock.)
				}
			}
		}

		sampleMatch.round = 0;
		sampleMatch.level = "Test Level";

		return sampleMatch;
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