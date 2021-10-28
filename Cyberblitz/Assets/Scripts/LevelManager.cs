using System;
using System.Collections.Generic;
using UnityEngine;

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
			levelDataDict.Add(levelData.name, levelData);
		}
	}

	public void LoadLevel(string name)
	{
		Debug.Log("Loading level");
		if (levelDataDict.TryGetValue(name, out LevelData levelData))
		{
			currentLevelData = levelData;
			currentLevel = Instantiate(levelData.levelPrefab, levelContainer);

			currentLevel.SetupLevel();

			Debug.Log($"Loaded Level: {levelData.name}");
			OnLevelLoaded?.Invoke(currentLevel);
		} else
		{
			Debug.LogWarning($"Could not find a LevelData with the name \"{name}\"");
		}
	}

	public void UnloadLevel()
	{

		if (currentLevel != null) Destroy(currentLevel);
		currentLevel = null;
		currentLevelData = null;

		OnLevelUnloaded?.Invoke();
	}
}
