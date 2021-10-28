using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "new_LevelDataList", menuName = "Level/DataList")]
public class LevelDataList : ScriptableObject
{
    public LevelData[] levelDatas;
}
