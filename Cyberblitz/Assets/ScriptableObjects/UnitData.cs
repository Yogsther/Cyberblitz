using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType
{
	Scout,
	Sniper,
	Heavy,
	Courier,
	Solider
}

[CreateAssetMenu(fileName = "new_UnitData", menuName = "Unit/Data")]
public class UnitData : ScriptableObject
{
	public UnitType type;
	public new string name;
	public UnitStats stats;
	[TextArea] public string description;
	[TextArea] public string story;
	public bool canGuard = true;
	public GameObject model;
	public GameObject image;
	public Sprite portrait, roleIcon;

	public AudioClip[] fireSounds;

	public AudioClip GetRandomFireSound()
    {
		return fireSounds[Random.Range(0, fireSounds.Length)];
	}
}
