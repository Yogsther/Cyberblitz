using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
	public static Match match;
	public TimelineEditor timelineEditor;
	public static MatchManager Instance;
	public LevelManager levelManager;

	private void Awake()
	{
		if (Instance != null) Destroy(this);
		else Instance = this;
	}

	public static bool IsInGame => match != null;

	public static Action<Match> OnMatchStart;
	public static Action<Match> OnMatchUpdate;
	public static Action OnPlanningStart;
	public static Action OnPlanningEnd;
	public static Action<Match, string> OnMatchEnd;
	public static Action OnMatchUnloaded;
	public static Action<MapVotes> OnMapVote;

	public void Init()
	{
		ClientConnection.On("MATCH_UPDATE", MatchUpdate);
		ClientConnection.On("SEND_UNITS", SendUnits);
		ClientConnection.On("GAME_TERMINATED", OnGameEnd);


		ClientConnection.On("DISCONNECTED", packet =>
		{
			if (match != null) UnloadMatch();
			ClientConnection.OnDisconnected?.Invoke();
		});

		TurnPlayback.OnPlaybackFinished += SignalReady;
		LevelManager.OnLevelLoaded += (level) => SignalReady();
	}

	public void OnSendUnitSelection()
	{
		ClientConnection.Emit("UNIT_SELECTION", PlayPage.GetSelectedUnits());
	}

	public void UnloadMatch()
	{
		match = null;
		levelManager.UnloadLevel();
		OnMatchUnloaded?.Invoke();
		ClientConnection.Emit("USER_LIST_UPDATE");
	}

	void OnGameEnd(NetworkPacket packet)
	{
		string reason = packet.content;
		Debug.Log("Match undefined? " + match);
		OnMatchEnd.Invoke(match, reason);
		Debug.LogWarning("Game terminated: " + reason);
	}

	void MatchUpdate(NetworkPacket packet)
	{
		match = packet.Parse<Match>();
		Debug.Log($"[Got match update] State: {match.state}");

		switch (match.state)
		{
			case Match.GameState.WaitingForUnitSelection:
				OnSendUnitSelection();
				break;
			case Match.GameState.Planning:
				QueueSystem.Call("MATCH_PLANNING");
				OnPlanningStart?.Invoke();
				break;
			case Match.GameState.Playback:
				QueueSystem.Call("MATCH_PLAYBACK");
				break;
			case Match.GameState.Starting:
				MatchStart();
				break;
			case Match.GameState.MapVote:
				OnMapVote(match.votes);
				break;
		}

		OnMatchUpdate?.Invoke(match);
	}

	void SendUnits(NetworkPacket packet)
	{
		OnPlanningEnd?.Invoke();
		Debug.Log("Sending units");
		ClientConnection.Emit("UNITS", match);
	}

	void MatchStart()
	{
		OnMatchStart?.Invoke(match);

		QueueSystem.Call("MATCH_START");

		// Load level...
		Debug.Log($"[Loading level] '{match.level}'");
	}

	public static void SignalReady()
	{
		Debug.Log("[Sent Ready]");
		ClientConnection.Emit("READY");
	}


	public static bool TryGetLocalPlayer(out Player localPlayer)
	{
		localPlayer = null;

		foreach (Player player in match.players)
		{
			if (player.user.id == ClientLogin.user.id)
			{
				localPlayer = player;
			}
		}

		return localPlayer != null;
	}

	public static Unit GetUnit(UnitID id)
	{
		if (IsInGame)
		{
			Unit[] units = match.GetAllUnits();

			for (int i = 0; i < units.Length; i++)
			{
				Unit unit = units[i];

				if (unit.id == id) return unit;
			}

			Debug.LogWarning($"Could not find a unit with the id {id}");
		} else
		{
			Debug.LogWarning($"No match started yet");
		}

		return null;
	}
}

