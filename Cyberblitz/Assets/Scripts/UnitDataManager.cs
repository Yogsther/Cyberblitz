using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDataManager : MonoBehaviour
{
    public UnitDataList unitDataList;

    public static Dictionary<UnitType, UnitData> unitDataDict = new Dictionary<UnitType, UnitData>();

    private void Awake()
    {
        foreach (UnitData unitData in unitDataList.unitDatas) unitDataDict.Add(unitData.type, unitData);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="type">The UnitType you want to get the UnitData for.</param>
    /// <returns>UnitData matching the input UnitType. null if no such UnitData can be found</returns>
    public static UnitData GetUnitDataByType(UnitType type)
    {
        if (unitDataDict.TryGetValue(type, out UnitData result))
        {
            return result;
        }

        Debug.LogWarning($"Could not find UnitData of type {type}");

        return null;
    }
}
