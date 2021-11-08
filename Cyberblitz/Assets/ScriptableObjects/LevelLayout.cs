using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new_LevelLayout", menuName = "Level/Layout")]
public class LevelLayout : ScriptableObject
{
	public string id;
	public new string name;
	public Vector2Int levelGridSize;
	public SpawnArea[] spawnAreas;
	public GridCollider[] gridColliders;
}