using Newtonsoft.Json;
using System;
using System.Collections;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
	public static Match match;
	public TimelineEditor timelineEditor;
	public static MatchManager Instance;


	private void Awake()
	{
		if (Instance != null) Destroy(this);
		else Instance = this;
	}

	public static bool IsInGame => match != null;

	public static Action<Match> OnMatchStart;
	public static Action<Match> OnMatchUpdate;
	public static Action OnPlanningEnd;

	public void Init()
	{
		ClientConnection.On("MATCH_UPDATE", MatchUpdate);
		ClientConnection.On("SEND_UNITS", SendUnits);
		ClientConnection.On("GAME_TERMINATED", OnGameEnd);

		TurnPlayback.OnPlaybackFinished += SignalReady;
		LevelManager.OnLevelLoaded += (level) => SignalReady();
	}

	void OnGameEnd(NetworkPacket packet)
	{
		string reason = packet.content;
		Debug.LogWarning("Game terminated: " + reason);
	}

	void MatchUpdate(NetworkPacket packet)
	{
		match = packet.Parse<Match>();

		Debug.Log("Match manager got match update: " + match.state.ToString());

		switch (match.state)
		{
			case Match.GameState.Planning:
				QueueSystem.Call("MATCH_PLANNING");
				break;
			case Match.GameState.Playback:
				Debug.Log("Stating match playback");
				QueueSystem.Call("MATCH_PLAYBACK");
				break;
			case Match.GameState.Starting:
				MatchStart();
				break;
		}

		OnMatchUpdate?.Invoke(match);
	}

	void SendUnits(NetworkPacket packet)
	{
		OnPlanningEnd?.Invoke();
		ClientConnection.Emit("UNITS", match);
	}


	void MatchStart()
	{
		Debug.Log("New match started");
		OnMatchStart?.Invoke(match);

		QueueSystem.Call("MATCH_START");

		// Load level...
		Debug.Log($"Loading level '{match.level}'...");
	}

	public static void SignalReady()
	{
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

