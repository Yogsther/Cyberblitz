using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new_LevelData", menuName = "Level/Data")]
public class LevelData : ScriptableObject
{
	public string id;
	public new string name;
	[TextArea] public string description;
	public Sprite thumbnail;

	public Level levelPrefab;
	public LevelLayout layout;
	public AudioClip music;
	public AudioClip ambience;
}
