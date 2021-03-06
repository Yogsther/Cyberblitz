using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VisualUnitManager : MonoBehaviour
{

	public GameObject visualUnitPrefab;

	private Transform visualUnitContainer;

	public static List<VisualUnit> visualUnits = new List<VisualUnit>();
	private static Dictionary<UnitID, VisualUnit> visualUnitDict = new Dictionary<UnitID, VisualUnit>();
	private static Dictionary<UserID, List<VisualUnit>> userVisualUnitsDict = new Dictionary<UserID, List<VisualUnit>>();

	int selectedUnitIndex = 0;

	public static Action OnVisualUnitsSpawned;

	private void Awake()
	{
		visualUnitContainer = new GameObject("Visual Unit Container").transform;
		MatchManager.OnMatchUnloaded += () =>
		{
			DeleteAllVisualUnits();
		};
	}

	/// <summary>
	/// Select the next unit based on selected unit by index
	/// So forward would select the next in the list, otherwise it would select the previous one in list
	/// </summary>
	void SelectNextUnit(bool next)
	{
		selectedUnitIndex += next ? 1 : -1;
		if (selectedUnitIndex > -1)
			selectedUnitIndex = selectedUnitIndex % (visualUnits.Count / 2);
		else selectedUnitIndex = (visualUnits.Count / 2) - 1;

		visualUnits[selectedUnitIndex].SetSelected(true);
	}

	private void Update()
	{
		if (MatchManager.match != null && MatchManager.match.state == Match.GameState.Planning)
		{
			if (Keyboard.current[Key.X].wasPressedThisFrame) SelectNextUnit(true);
			if (Keyboard.current[Key.Z].wasPressedThisFrame) SelectNextUnit(false);
		}
	}

	private void Start()
	{
		TimelineEditor.OnUnitSelected += unit =>
		{
			selectedUnitIndex = visualUnits.IndexOf(GetVisualUnitById(unit));
			Debug.Log("Selected unit index: " + selectedUnitIndex);
		};

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
			if (match.IsOwnerOfUnit(ClientLogin.user.id, visualUnit.id) && !visualUnit.isDead)
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
					visualUnitInstance.name = $"VisualUnit {(friendlyPlayer ? "Friendly" : "Enemy")} - {unitData.type} - {unit.id}";
					visualUnitInstance.id = unit.id;
					visualUnitInstance.friendly = friendlyPlayer;
					visualUnitInstance.model = Instantiate(unitData.model, visualUnitInstance.modelTransform);
					visualUnitInstance.model.transform.localPosition = Vector3.zero;
					visualUnitInstance.model.transform.localRotation = Quaternion.AngleAxis(visualUnitInstance.rotationOffset, Vector3.down);

					visualUnitInstance.rigidBodies = visualUnitInstance.model.GetComponentsInChildren<Rigidbody>();
					/*visualUnitInstance.rigidBodies.AddRange();*/


					if (friendlyPlayer) visualUnitInstance.model.layer = 7;
					else
					{
						ReplaceChildLayersRecursive(visualUnitInstance.transform, 7, 13);
					}
					visualUnitInstance.overheadIconRenderer.sprite = unitData.roleIcon;
					if (unit.isMVP) visualUnitInstance.overheadIconRenderer.gameObject.SetActive(true);
					//visualUnitInstance.overheadIconRenderer.color = 

					visualUnitInstance.isSelectable = false;

					visualUnitInstance.modelTransform.position = unit.position.ToVector2().ToFlatVector3();
					visualUnitInstance.SetTargetForward(spawnArea.cameraRotation * Vector3.forward);

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

	public void SetChildLayersRecursive(Transform parent, int layer)
    {
		parent.gameObject.layer = layer;
		foreach (Transform child in parent.GetComponentsInChildren<Transform>())
        {
			child.gameObject.layer = layer;
        }
    }

	public void ReplaceChildLayersRecursive(Transform parent, int oldLayer, int newLayer)
	{
		if(parent.gameObject.layer == oldLayer)
        {
			parent.gameObject.layer = newLayer;
		}
		
		foreach (Transform child in parent.GetComponentsInChildren<Transform>())
		{
			if(child.gameObject.layer == oldLayer)
			child.gameObject.layer = newLayer;
		}
	}

	private void DeleteAllVisualUnits()
	{
		selectedUnitIndex = 0;
		foreach (VisualUnit visualUnit in visualUnits)
		{
			Debug.Log("Destroyed visual unit");
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
