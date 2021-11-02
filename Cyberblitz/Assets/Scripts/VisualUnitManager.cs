using System;
using System.Collections.Generic;
using UnityEngine;

public class VisualUnitManager : MonoBehaviour
{

	public GameObject visualUnitPrefab;

	private Transform visualUnitContainer;

	public static List<VisualUnit> visualUnits = new List<VisualUnit>();
	private static Dictionary<UnitID, VisualUnit> visualUnitDict = new Dictionary<UnitID, VisualUnit>();
	private static Dictionary<UserID, List<VisualUnit>> userVisualUnitsDict = new Dictionary<UserID, List<VisualUnit>>();


	public static Action OnVisualUnitsSpawned;

	private void Awake()
	{
		visualUnitContainer = new GameObject("Visual Unit Container").transform;
	}

	private void Start()
	{
		/*QueueSystem.Subscribe("MATCH_START", () =>
		{
			
		});*/

		LevelManager.OnLevelLoaded += (level) =>
		{
			SpawnUnits(level);
		};

		MatchManager.OnMatchUpdate += OnMatchUpdate;

	}

	void OnMatchUpdate(Match match)
	{
		SetFriendlyUnitsSelectable(match.state == Match.GameState.Planning);
	}

	void SetFriendlyUnitsSelectable(bool selectable)
	{
		Match match = MatchManager.match;
		foreach (VisualUnit visualUnit in visualUnits)
			if (match.IsOwnerOfUnit(ClientLogin.user.id, visualUnit.id))
				visualUnit.isSelectable = selectable;
	}

	public void SpawnUnits(Level level)
	{
		DeleteAllVisualUnits();

		foreach (Player player in MatchManager.match.players)
		{
			int team = player.team;

			SpawnArea spawnArea = level.spawnAreas[team];
			bool friendlyPlayer = player.user.id == ClientLogin.user.id;

			List<VisualUnit> userVisualUnits = new List<VisualUnit>();
			foreach (Unit unit in player.units)
			{
				UnitData unitData = UnitDataManager.GetUnitDataByType(unit.type);

				if (unitData != null)
				{
					GameObject go = Instantiate(visualUnitPrefab, visualUnitContainer);
					VisualUnit visualUnitInstance = go.GetComponent<VisualUnit>();
					visualUnitInstance.name = $"VisualUnit {(friendlyPlayer ? "Friendly" : "Enemy")} - {unitData.type.ToString()} - {unit.id}";
					visualUnitInstance.id = unit.id;
					Instantiate(unitData.model, visualUnitInstance.mainModel);
					Instantiate(unitData.model, visualUnitInstance.ghostModel);

					visualUnitInstance.isSelectable = false;

					visualUnitInstance.mainModel.position = unit.position.ToVector2().ToFlatVector3();
					visualUnitInstance.mainModel.rotation = spawnArea.cameraRotation;

					userVisualUnits.Add(visualUnitInstance);
				}
			}

			visualUnits.AddRange(userVisualUnits);
			userVisualUnitsDict.Add(player.user.id, userVisualUnits);
		}

		foreach (VisualUnit visualUnit in visualUnits)
		{
			visualUnitDict.Add(visualUnit.id, visualUnit);
		}

		OnVisualUnitsSpawned?.Invoke();
	}

	private void DeleteAllVisualUnits()
	{
		foreach (VisualUnit visualUnit in visualUnits)
		{
			Destroy(visualUnit.gameObject);
		}

		ClearAllListsAndDicts();
	}

	private void ClearAllListsAndDicts()
	{
		visualUnits.Clear();
		visualUnitDict.Clear();
		userVisualUnitsDict.Clear();
	}

	public static VisualUnit GetVisualUnitById(UnitID id)
	{
		if (visualUnitDict.TryGetValue(id, out VisualUnit result))
		{
			return result;
		}

		Debug.Log($"Could not find VisualUnit with id {id}");

		return null;
	}

	public static List<VisualUnit> GetUserVisualUnitsById(UserID id)
	{
		if (!userVisualUnitsDict.TryGetValue(id, out List<VisualUnit> result))
		{
			Debug.Log($"Could not find VisualUnits belonging to user with id {id}");
		}

		return result;
	}

}
