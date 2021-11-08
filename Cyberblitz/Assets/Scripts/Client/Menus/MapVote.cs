using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapVote : MonoBehaviour
{

	public Transform mapParent;
	public MapVoteEntry mapPrefab;

	private void Awake()
	{

	}

	private void Start()
	{
		LoadMaps();
	}

	public void LoadMaps()
	{
		ClearMaps();
		foreach (LevelData levelData in LevelManager.levelDataDict.Values)
		{
			MapVoteEntry entry = Instantiate(mapPrefab, mapParent);
			entry.Setup(levelData);
		}
	}

	void ClearMaps()
	{
		foreach (Transform child in mapParent)
		{
			Destroy(child.gameObject);
		}
	}

	void Update()
	{

	}
}
