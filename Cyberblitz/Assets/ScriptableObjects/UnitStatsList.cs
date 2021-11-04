using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new_UnitStatsList", menuName = "Unit/StatsList")]
public class UnitStatsList : ScriptableObject
{
    public List<UnitStats> unitStats;

    private Dictionary<UnitType, UnitStats> statDict = new Dictionary<UnitType, UnitStats>();
    private bool hasBeenCompiled = false;
    public void Compile()
    {
        statDict.Clear();

        foreach (UnitStats stats in unitStats)
        {
            statDict.Add(stats.unitType, stats);
        }


        hasBeenCompiled = true;
    }

    public bool TryGetUnitStatsByType(UnitType type, out UnitStats stats)
    {
        stats = null;

        if (hasBeenCompiled)
        {
            statDict.TryGetValue(type, out stats);
        }
        else
        {
            Debug.Log($"[UnitStatsList] - List has not been compiled. You need to call UnitStatsList.Compile() first.");
        }

        return stats != null;
    }
}
