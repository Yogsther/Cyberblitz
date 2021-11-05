using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static LevelLayoutList levelLayouts;
    public static UnitStatsList unitStats;

    public LevelLayoutList levelLayoutsToLoad;
    public UnitStatsList unitStatsToLoad;

    private void Awake()
    {
        levelLayouts = levelLayoutsToLoad;
        unitStats = unitStatsToLoad;


        levelLayouts.Compile();
        unitStatsToLoad.Compile();
    }
}
