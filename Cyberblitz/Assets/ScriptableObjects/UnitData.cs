using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType
{
    Scout,
    Sniper,
    Heavy
}

[System.Serializable]
public class UnitStats
{
    public float maxHp = 2f;
    public float speed = 2f;
    public float damage = 2f;
    public float firerate = 2f;
    public float range = 5f;
    public float spread = 45f;
}


[CreateAssetMenu(fileName = "new_UnitData", menuName = "Unit/Data")]
public class UnitData : ScriptableObject
{
    public UnitType type;
    public new string name;
    public UnitStats stats;
    [TextArea] public string description;
    public bool canGuard = true;
    public GameObject model;
    public Sprite image;
}
