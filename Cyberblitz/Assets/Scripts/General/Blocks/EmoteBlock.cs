using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmoteBlock : Block
{

	public EmoteBlock(UnitID ownerId, int timelineIndex) : base(ownerId, timelineIndex)
	{
		type = BlockType.Emote;
	}
	public override void Simulate(Match simulatedMatch, float localTime)
	{

	}

	public override void OnPlaybackStart(Match simulatedMatch)
	{
		VisualUnit unit = VisualUnitManager.GetVisualUnitById(ownerId);
		unit.animator.SetTrigger("Emote");
	}

	public override void Playback(Match simulatedMatch, float localTime)
	{

	}

	public override void OnPlaybackEnd(Match simulatedMatch)
	{
		Debug.Log("Stopping emote");
		VisualUnit unit = VisualUnitManager.GetVisualUnitById(ownerId);
		/*unit.animator.ResetTrigger("Emote");*/
		unit.animator.SetTrigger("StopEmote");
	}
}