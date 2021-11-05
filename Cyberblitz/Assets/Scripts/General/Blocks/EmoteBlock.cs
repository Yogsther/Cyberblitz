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

	}

	public override void Playback(Match simulatedMatch, float localTime)
	{

	}

}