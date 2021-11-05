using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new_UnitStats", menuName = "Unit/Stats")]
public class UnitStats : ScriptableObject
{
	public UnitType unitType;

	public float maxHp = 2f;
	public float speed = 2f;
	public float damage = 2f;
	// Shots per second
	public float firerate = 2f;
	public float range = 5f;
	public float spread = 45f;
}
