using UnityEngine;

public class TurnSimulator : MonoBehaviour
{

    public static Match SimulateTurn(Match match)
    {
        match.longestTimelineDuration = 0f;


        foreach (Unit unit in match.GetAllUnits())
        {
            float timelineDuration = unit.timeline.GetDuration();
            if (timelineDuration > match.longestTimelineDuration)
            {
                match.longestTimelineDuration = timelineDuration;
            }
        }

        Debug.Log("[TurnSimulator] - Started simulation");

        for (float time = 0f; time < match.longestTimelineDuration; time += 1f / match.rules.simulationTickrate)
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
