using System;
using System.Collections.Generic;
using UnityEngine;
using WayStarPathfinding;

public class LevelManager : MonoBehaviour
{
	public static LevelManager instance { get; private set; }

	public bool showGameObjects = true;

	public LevelDataList levelDataList;
	public static LevelData[] levelDatas;
	public static Dictionary<string, LevelData> levelDataDict = new Dictionary<string, LevelData>();

	public LevelData currentLevelData { get; private set; }
	public Level currentLevel;

	private Transform levelContainer;

	public static Action<Level> OnLevelLoaded;
	public static Action OnLevelUnloaded;

	private void Awake()
	{
		if (instance != null)
		{
			Destroy(this);
		} else
		{
			instance = this;
		}


		UpdateLevelDataDict();

		levelContainer = new GameObject("Level Container").transform;

		/*MatchManager.OnMatchStart += match => LoadLevel(match.level);*/
	}

	private void Start()
	{

		OnLevelLoaded += (level) =>
		{
			PathFinder.gridSize = level.levelGridSize;
			Debug.Log($"[LevelManager] - Level loaded: {level.name}");
		};

		QueueSystem.Subscribe("MATCH_START", () =>
		{
			LoadLevel(MatchManager.match.level);
		});

	}

	private void UpdateLevelDataDict()
	{
		levelDataDict = new Dictionary<string, LevelData>();
		foreach (LevelData levelData in levelDataList.levelDatas)
		{
			levelDataDict.Add(levelData.id, levelData);
		}
	}

	public void LoadLevel(string id)
	{
		UnloadLevel();

		Debug.Log($"[LevelManager] - Loading level: {id}");
		if (levelDataDict.TryGetValue(id, out LevelData levelData))
		{
			currentLevelData = levelData;
			currentLevel = Instantiate(levelData.levelPrefab, levelContainer);

			currentLevel.SetupLevel();

			SoundManager.PlayAmbience(levelData.ambience);
			SoundManager.PlayMusic(levelData.music);

			OnLevelLoaded?.Invoke(currentLevel);
		} else
		{
			Debug.LogWarning($"[LevelManager] - Could not find a LevelData with the id \"{id}\"");
		}
	}

	public void UnloadLevel()
	{
		if (currentLevel != null) Destroy(currentLevel.gameObject);
		currentLevel = null;
		currentLevelData = null;

		Debug.Log("Destroyed current level!");

		OnLevelUnloaded?.Invoke();
	}
}
