using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new_UnitDataList", menuName = "Unit/DataList")]
public class UnitDataList : ScriptableObject
{
    public List<UnitData> unitDatas;
}
