using Newtonsoft.Json;
using System;
using System.Collections;
using UnityEngine;

public class TurnPlayback : MonoBehaviour
{


	public static Action OnPlaybackStarted;
	public static Action OnPlaybackFinished;


	private void Start()
	{
		QueueSystem.Subscribe("MATCH_PLAYBACK", () => StartTurnPlayback(MatchManager.match));
	}



	/*[ContextMenu("TestStartGame")]
	public void StartTestMatch()
	{
		MatchManager.OnMatchStart.Invoke(MatchManager.match);
	}*/
	/*ClientConnection.Emit("START_MATCH");*/


	[ContextMenu("TestPlayback")]
	public void TestPlayback()
	{
		if (Application.isPlaying)
		{

			StartTurnPlayback(MatchManager.match);
		}
	}

	public void StartTurnPlayback(Match match)
	{
		StartCoroutine(Playback(match));
	}


	public IEnumerator Playback(Match match)
	{
		yield return new WaitForSeconds(.2f);

		Debug.Log("Started Playback");
		OnPlaybackStarted?.Invoke();


		Unit[] units = match.GetAllUnits();

		for (float time = 0f; time < match.rules.turnTime; time += Time.deltaTime)
		{
			foreach (Unit unit in units)
			{

				if (unit.timeline.TryGetBlockAtTime(time, out Block block))
				{

					float blockStartTime = unit.timeline.GetStartTimeOfBlockAtIndex(block.timelineIndex);

					float blockLocalTime = time - blockStartTime;

					block.Playback(match, blockLocalTime);
				} else
				{
					//Debug.LogWarning($"Unit {unit.id} had a Block that was null at time {time}");
				}
			}

			yield return null;


		}

		Debug.Log("Finished Playback");

		OnPlaybackFinished?.Invoke();
	}
}
