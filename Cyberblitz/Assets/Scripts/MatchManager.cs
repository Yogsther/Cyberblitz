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
		/*foreach (Player player in match.players)
		{
			foreach (Unit unit in player.units)
			{
				if (unit.timeline.GetSize() > 0)
				{
					Debug.Log("Player " + player.team + " , unit : " + unit.id);
					Debug.Log(unit.timeline.GetSize());
					MoveBlock block = (MoveBlock)unit.timeline.blocks[0];
					foreach (Vector2Int point in block.movementPath.GetPoints())
					{
						Debug.Log("POINT: " + point);
					}

					NetworkPacket test_packet = new NetworkPacket("test", unit);
					Debug.Log(test_packet.content);
				}

			}
		}*/

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


	public static void SignalReady()
	{
		ClientConnection.Emit("READY");
	}
}

