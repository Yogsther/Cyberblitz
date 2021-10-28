using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSimulator : MonoBehaviour
{

	public static Match SimulateTurn(Match match)
	{
		Debug.Log("[TurnSimulator] - Started simulation");

		for (float time = 0f; time < match.rules.turnTime; time += 1f / match.rules.simulationTickrate)
		{
			foreach (Unit unit in match.GetAllUnits())
			{

				Block block = unit.timeline.GetBlockAtTime(time);
				if (block != null)
				{
					float blockStartTime = unit.timeline.GetStartTimeOfBlockAtIndex(block.timelineIndex);

					float blockLocalTime = time - blockStartTime;

					block.Simulate(match, blockLocalTime);
				}


			}
		}

		Debug.Log("[TurnSimulator] - Finished simulation");

		return match;

	}


}
