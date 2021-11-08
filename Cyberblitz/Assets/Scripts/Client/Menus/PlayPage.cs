using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class PlayPage : MonoBehaviour
{
	public MenuSystem menuSystem;
	public AddUnitPanel[] panels;
	public Transform modelSpawns;
	public Button playButton;
	public PlayerBrowser playerBrowser;

	public RuntimeAnimatorController selectAnimationController;

	public VisualEffect[] spawnEffects;


	class SelectedUnit
	{
		public UnitType type;
		public bool loaded = false;
		public bool empty = true;
		public SelectedUnit(UnitType type)
		{
			this.type = type;
		}
	}

	static SelectedUnit[] selectedUnits = new SelectedUnit[3];

	private void Awake()
	{
		menuSystem.OnPageLoad["play"] = OnPageLoad;
		MatchManager.OnMatchUnloaded += () =>
		{
			foreach (SelectedUnit selectedUnit in selectedUnits)
			{
				selectedUnit.loaded = false;
			}
			LoadModels();
		};
	}

	private void Start()
	{
		playButton.onClick.AddListener(() =>
		{
			if (HasSelectedUnits())
			{
				playerBrowser.SetVisbility(true);
			}
		});
	}

	void ClearModelsInSpawnpoint(Transform spawnpoint)
	{
		foreach (Transform model in spawnpoint)
			Destroy(model.gameObject);
	}

	public static bool HasSelectedUnits()
	{
		foreach (SelectedUnit selectedUnit in selectedUnits)
			if (selectedUnit == null || selectedUnit.empty) return false;
		return true;
	}

	public static UnitType[] GetSelectedUnits()
	{
		UnitType[] types = new UnitType[3];
		for (int i = 0; i < selectedUnits.Length; i++) types[i] = selectedUnits[i].type;
		return types;
	}

	public void SelectUnit(UnitType type, int index)
	{
		selectedUnits[index] = new SelectedUnit(type);
		selectedUnits[index].empty = false;
		LoadModels();
	}

	void LoadModels()
	{
		for (int i = 0; i < selectedUnits.Length; i++)
		{
			SelectedUnit selectedUnit = selectedUnits[i];
			if (selectedUnit != null && !selectedUnit.loaded && !selectedUnit.empty)
			{
				ClearModelsInSpawnpoint(modelSpawns.GetChild(i));
				GameObject model = Instantiate(UnitDataManager.GetUnitDataByType(selectedUnit.type).model, modelSpawns.GetChild(i));
				Animator animator = model.GetComponent<Animator>();
				animator.runtimeAnimatorController = selectAnimationController;
				animator.applyRootMotion = true;
				spawnEffects[i].enabled = false;

				selectedUnit.loaded = true;

			}
		}
	}



	void OnPageLoad()
	{
		foreach (AddUnitPanel panel in panels)
		{
			panel.playPage = this;
			panel.ClosePanel();
		}
		menuSystem.menuBackground.SetActive(false);
	}

	public void OpenPanel(AddUnitPanel panelToOpen)
	{
		foreach (AddUnitPanel panelToClose in panels) panelToClose.ClosePanel();
		panelToOpen.OpenPanel();
	}

	void Update()
	{

	}
}
