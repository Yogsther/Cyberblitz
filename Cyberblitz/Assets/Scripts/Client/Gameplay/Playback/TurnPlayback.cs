using System;
using System.Collections;
using System.Collections.Generic;
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

        Dictionary<UnitID, BlockID> activeBlocks = new Dictionary<UnitID, BlockID>();

        Unit[] units = match.GetAllUnits();

        MatchEvent nextEvent = null;

        Debug.Log($"[Starting Playback] EventCount: {match.events.Count}");

        if (match.events.Count != 0)
        {
            nextEvent = match.events.Dequeue();
        }

        bool startedMusic = false;

        for (float time = -1f; time < match.longestTimelineDuration + 1f; time += Time.deltaTime)
        {

            if (time >= match.longestTimelineDuration && match.winner != null && !startedMusic)
            {
                startedMusic = true;
                SoundManager.PlayMusicNonLooping($"{(match.winner == ClientLogin.user.id ? "winning" : "losing")}_music");
            }

            if (nextEvent != null)
            {
                if (time + 1f > nextEvent.time && !nextEvent.hasDonePre)
                {
                    nextEvent.PreEffect();
                }

                if (time > nextEvent.time)
                {
                    nextEvent.PlaybackEffect(match);

                    nextEvent.PostEffect();
                    if (match.events.Count != 0)
                    {
                        nextEvent = match.events.Dequeue();
                    }
                    else
                    {
                        nextEvent = null;
                    }
                }
            }

            foreach (Unit unit in units)
            {
                if (!VisualUnitManager.GetVisualUnitById(unit.id).isDead)
                {

                    if (unit.timeline.TryGetBlockAtTime(time, out Block block))
                    {
                        // Check if the playing block is the same as last frame
                        if (!activeBlocks.ContainsKey(unit.id) || activeBlocks[unit.id] != block.id)
                        {
                            if (activeBlocks.ContainsKey(unit.id))
                            {
                                unit.timeline.GetBlock(activeBlocks[unit.id]).OnPlaybackEnd(match);
                            }

                            block.OnPlaybackStart(match);
                            activeBlocks[unit.id] = block.id;
                        }


                        float blockStartTime = unit.timeline.GetStartTimeOfBlockAtIndex(block.timelineIndex);

                        float blockLocalTime = time - blockStartTime;

                        block.Playback(match, blockLocalTime);
                    }
                    else
                    {
                        //Debug.LogWarning($"Unit {unit.id} had a Block that was null at time {time}");
                    }
                }

            }
            yield return null;
        }

        // Finish all started blocks at the end of the timeline
        foreach (UnitID unitId in activeBlocks.Keys)
        {
            match.GetUnit(unitId).timeline.GetBlock(activeBlocks[unitId]).OnPlaybackEnd(match);
        }

        Debug.Log("[Finished playback]");
        OnPlaybackFinished?.Invoke();
    }
}
