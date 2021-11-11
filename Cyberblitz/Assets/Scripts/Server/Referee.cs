using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Referee
{
	public Match match;

	public LevelLayout levelLayout;
	List<UserID> readyPlayers = new List<UserID>();

	void MatchStep()
	{
		ClearReadyPlayers();

		switch (match.state)
		{
			case Match.GameState.WaitingForUnitSelection:
				match.state = Match.GameState.MapVote;
				break;
			case Match.GameState.MapVote:
				StartGame();
				break;
			case Match.GameState.Starting:
				match.state = Match.GameState.Planning;
				break;
			case Match.GameState.Playback:
				PrepareForPlanning();
				match.state = Match.GameState.Planning;
				break;
			case Match.GameState.Planning:
				match.state = Match.GameState.WaitingForUnits;
				break;
			case Match.GameState.WaitingForUnits:
				match.state = Match.GameState.Playback;
				break;
		}

		Debug.Log("Starting new round, phase: " + match.state);

		if (match.state == Match.GameState.Playback)
			match = TurnSimulator.SimulateTurn(match);

		if (match.state == Match.GameState.Planning)
		{
			if (match.winner != null)
			{
				match.state = Match.GameState.Ending;
			}
		}


		if (match.state != Match.GameState.WaitingForUnits) SendGameUpdate();
		else Broadcast("SEND_UNITS");

		if (match.state == Match.GameState.Ending) Terminate(match.GetUser(match.winner) + " won the game.");

	}

	void PrepareForPlanning()
	{
		// Clear all unit timelines
		foreach (Player player in match.players)
			foreach (Unit unit in player.units)
				unit.timeline.Clear();


		// Set position to default
	}

	public void OnPlayerDisconnect(User disconnectedUser)
	{
		Terminate(disconnectedUser.username + " disconnected");
	}

	public void Terminate(string reason)
	{
		Broadcast("GAME_TERMINATED", reason);
		ServerCore.TerminateGame(match.id);
	}

	void OnPlayerReady(NetworkPacket packet)
	{
		Debug.Log("Got ready from player");
		// Conditions where players cannot set ready.
		if (match.state == Match.GameState.WaitingForUnits) return;

		SetPlayerReady(packet.user.id);
	}

	void SetPlayerReady(UserID id)
	{
		if (!readyPlayers.Contains(id)) readyPlayers.Add(id);

		// If all players are ready, go to next phase
		if (IsAllPlayersReady())
		{
			Debug.Log("All players ready");
			MatchStep();
		}
	}

	void StartListening()
	{
		ServerConnection.OnMatchContext(match.id, "READY", OnPlayerReady);
		ServerConnection.OnMatchContext(match.id, "UNITS", OnUnits);
		ServerConnection.OnMatchContext(match.id, "VOTE", OnVote);
		ServerConnection.OnMatchContext(match.id, "UNIT_SELECTION", OnUnitSelection);
	}

	void OnUnitSelection(NetworkPacket packet)
	{
		UnitType[] unitTypes = packet.Parse<UnitType[]>();
		foreach (Player player in match.players)
		{
			if (player.user.id == packet.user.id)
			{
				AddUnits(player, unitTypes);
				SetPlayerReady(player.user.id);
			}
		}
	}

	void OnVote(NetworkPacket packet)
	{
		Vote vote = packet.Parse<Vote>();

		// For security lol
		vote.user = packet.user.id;

		Debug.Log("Recorded vote from " + packet.user.username);

		match.votes.AddVote(vote);
		SendGameUpdate();
	}

	void OnUnits(NetworkPacket packet)
	{
		Match userMatch = packet.Parse<Match>();

		foreach (Player player in userMatch.players)
			if (player.user.id == packet.user.id)
				foreach (Unit unit in player.units)
					ReplaceUnitTimeline(unit.id, unit.timeline);

		SetPlayerReady(packet.user.id);
	}

	void ReplaceUnitTimeline(UnitID id, Timeline timeline)
	{
		match.GetUnit(id).timeline = timeline;
	}

	/// <summary>
	/// Sends message to all players in match
	/// </summary>
	void Broadcast(string message, object content)
	{
		foreach (Player player in match.players)
		{
			ServerCore.SendTo(player.user.id, message, content);
		}
	}

	/// <summary>
	/// Sends message to all players in match
	/// </summary>
	void Broadcast(string message)
	{
		Broadcast(message, "");
	}

	public void SendGameUpdate()
	{
		Broadcast("MATCH_UPDATE", match);
	}

	void ChooseMap()
	{
		int mostUpvotes = 0;
		List<string> mapContenders = new List<string>();

		foreach (LevelLayout level in DataManager.levelLayouts.layoutDict.Values)
		{
			if (match.votes.GetVotes(level.id, VoteType.Downvote) == 0)
			{
				int upvotes = match.votes.GetVotes(level.id, VoteType.Upvote);

				if (upvotes >= mostUpvotes)
				{
					if (upvotes > mostUpvotes)
					{
						mostUpvotes = upvotes;
						mapContenders.Clear();
					}
					mapContenders.Add(level.id);
				}
			}
		}

		if (mapContenders.Count == 0) match.level = "CITY";
		else match.level = mapContenders[UnityEngine.Random.Range(0, mapContenders.Count - 1)];

		Debug.Log("Map contenders count: " + mapContenders.Count);
	}

	void StartGame()
	{

		ChooseMap();

		Debug.Log("Trying to start with level: " + match.level);
		match.state = Match.GameState.Starting;
		DataManager.levelLayouts.TryGetLevelLayout(match.level, out levelLayout);
		Debug.Log("LevelLayout: ? " + levelLayout);
		SetUnitSpawnpoints();

		// Announce new game start in console
		List<string> playerNames = new List<string>();
		foreach (Player player in match.players) playerNames.Add(player.user.username);
		Debug.Log("Startin game with: " + String.Join(", ", playerNames.ToArray()));

		SendGameUpdate();
	}

	public void Init()
	{
		match = new Match();
		match.level = "CITY";

		match.state = Match.GameState.WaitingForUnitSelection;
		match.round = 0;

		match.players = new Player[2];

		StartListening();
	}

	public void AddPlayer(User user)
	{
		int team = GetFreePlayerTeam();
		match.players[team] = CreatePlayer(user, team);
	}

	public void AddBot()
	{
		User botUser = new User();
		botUser.bot = true;
		botUser.username = "Bot";
		int team = GetFreePlayerTeam();

		UnitType[] defaultUnits = new UnitType[] { UnitType.Sniper, UnitType.Scout, UnitType.Heavy };

		match.players[team] = CreatePlayer(botUser, team);
		AddUnits(match.players[team], defaultUnits);
	}

	int GetFreePlayerTeam()
	{
		// Start checking indexes from zero.
		int team = 0;
		// Check if the team slot is free
		while (team < match.players.Length && match.players[team] != null)
			team++; // Team slot is not free, go to next

		// Return the team slot if its free
		if (match.players[team] == null) return team;
		// Return -1 if no free team was found.
		return -1;
	}

	void AddUnit(Player player, UnitType type, int index)
	{
		Unit unit = new Unit(player.user.id);

		unit.type = type;

		DataManager.unitStats.TryGetUnitStatsByType(unit.type, out UnitStats unitStats);

		unit.isMVP = type == UnitType.Courier;
		unit.hp = unitStats.maxHp;
		unit.ownerID = player.user.id;

		player.units[index] = unit;
	}

	void SetUnitSpawnpoints()
	{
		foreach (Player player in match.players)
		{
			int team = player.team;
			SpawnArea spawnArea = levelLayout.spawnAreas[team];

			for (int i = 0; i < player.units.Length; i++)
				player.units[i].SetPosition(spawnArea.GetSpawnPosition(i));
		}
	}

	public void AddUnits(Player player, UnitType[] unitTypes)
	{
		// Not an elegant way of adding units.. :(
		AddUnit(player, UnitType.Courier, 0);
		for (int i = 0; i < unitTypes.Length; i++)
			AddUnit(player, unitTypes[i], i + 1);
	}

	Player CreatePlayer(User user, int team)
	{
		Player player = new Player(user, team);

		Debug.Log("Created player " + player.user.username + " on team " + team);
		return player;
	}

	void ClearReadyPlayers()
	{
		readyPlayers.Clear();
	}

	bool IsAllPlayersReady()
	{
		foreach (Player player in match.players)
			if (!player.user.bot && !readyPlayers.Contains(player.user.id))
				return false;

		return true;
	}


}
