using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Referee
{
	public Match match;
	public LevelData levelData;
	List<UserID> readyPlayers = new List<UserID>();

	void MatchStep()
	{

		switch (match.state)
		{
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


		if (match.state != Match.GameState.WaitingForUnits) SendGameUpdate();
		else Broadcast("SEND_UNITS");

		ClearReadyPlayers();
	}

	void PrepareForPlanning()
	{
		// Clear all unit timelines
		foreach (Player player in match.players)
			foreach (Unit unit in player.units)
				unit.timeline.Clear();


		// Set position to default
	}

	void NewRound()
	{
		match.round++;
	}

	public void OnPlayerDisconnect(User disconnectedUser)
	{
		Terminate(disconnectedUser.username + " disconnected");
	}

	public void Terminate(string reason)
	{
		Broadcast("GAME_TERMINATED", reason);
	}

	void OnPlayerReady(NetworkPacket packet)
	{
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

	void SendGameUpdate()
	{
		Broadcast("MATCH_UPDATE", match);
	}

	public void Start()
	{

		// Announce new game start in console
		List<string> playerNames = new List<string>();
		foreach (Player player in match.players) playerNames.Add(player.user.username);
		Debug.Log("Startin game with: " + String.Join(", ", playerNames.ToArray()));

		// Start the game
		ClearReadyPlayers();
		SendGameUpdate();
	}

	public void Init()
	{
		match = new Match();
		match.level = "Test Level";
		match.state = Match.GameState.Starting;
		match.round = 0;

		match.players = new Player[2];

		StartListening();
	}

	public void LoadLevel()
	{
		bool gotLevel = LevelManager.levelDataDict.TryGetValue(match.level, out levelData);

		if (gotLevel)
		{
			Debug.Log($"Loaded LevelData - {match.level}");
		} else
		{
			Debug.Log($"Failed to load LevelData - {match.level}");
		}
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
		match.players[team] = CreatePlayer(botUser, team);
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

	Player CreatePlayer(User user, int team)
	{
		Player player = new Player(user, team);
		player.units = new Unit[5];

		SpawnArea spawnArea = levelData.levelPrefab.spawnAreas[team];

		for (int i = 0; i < player.units.Length; i++)
		{
			Unit unit = new Unit(player.user.id);

			unit.type = (UnitType)(i % 3);
			UnitData unitData = UnitDataManager.GetUnitDataByType(unit.type);

			unit.isMVP = i == 3;
			unit.hp = unitData.stats.maxHp;
			unit.ownerID = player.user.id;

			Vector2Int spawnPos = spawnArea.GetSpawnPosition(i);

			unit.SetPosition(spawnPos.x, spawnPos.y);
			player.units[i] = unit;
		}

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
