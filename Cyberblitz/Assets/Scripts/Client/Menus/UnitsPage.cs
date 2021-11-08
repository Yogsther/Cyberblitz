using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UnitsPage : MonoBehaviour
{
	List<UnitType> unitTypeList;
	int selectedUnitIndex = 0;
	UnitType selectedUnitType;
	UnitData selectedUnitData;

	public MenuSystem menuSystem;

	public TMP_Text title, description, story;
	public Sprite dotFilled, dotEmpty;

	public Transform health, speed, damage;

	GameObject unitImageDisplay = null;



	public void SelectUnit(UnitType type)
	{
		for (int i = 0; i < unitTypeList.Count; i++)
			if (unitTypeList[i] == type)
			{
				selectedUnitIndex = i;
				LoadUnit();
			}
	}

	void Awake()
	{
		unitTypeList = new List<UnitType>(UnitDataManager.unitDataDict.Keys);
		if (unitTypeList.Count == 0) Debug.LogError("Unit list is empty!");
		else selectedUnitType = unitTypeList[selectedUnitIndex];
		LoadUnit();

		MenuSystem.OnScreenLoaded += name =>
		{
			if (name == "units") LoadUnit();
		};
	}

	void SetDots(float value, Transform dots)
	{
		for (int i = 0; i < dots.childCount; i++)
			dots.GetChild(i).GetComponent<Image>().sprite = value >= (i + 1) ? dotFilled : dotEmpty;
	}

	public void Navigate(bool forward)
	{
		selectedUnitIndex = (selectedUnitIndex + (forward ? 1 : -1)) % unitTypeList.Count;
		if (selectedUnitIndex == -1) selectedUnitIndex = unitTypeList.Count - 1;

		LoadUnit();
	}

	void LoadUnit()
	{
		menuSystem.ClearSubHeader();
		foreach (UnitType unitType in unitTypeList)
		{
			UnitData unitData = UnitDataManager.GetUnitDataByType(unitType);
			menuSystem.CreateSubHeaderButton(unitData.name, () =>
			{
				SelectUnit(unitType);
			});
		}

		selectedUnitType = unitTypeList[selectedUnitIndex];

		selectedUnitData = UnitDataManager.GetUnitDataByType(selectedUnitType);
		if (unitImageDisplay) DestroyImmediate(unitImageDisplay);
		unitImageDisplay = Instantiate(selectedUnitData.image, transform);
		unitImageDisplay.gameObject.SetActive(true);

		title.text = selectedUnitData.name;
		description.text = selectedUnitData.description;
		story.text = selectedUnitData.story;

		SetDots(selectedUnitData.stats.maxHp, health);
		SetDots(selectedUnitData.stats.speed, speed);
		SetDots((selectedUnitData.stats.damage * selectedUnitData.stats.firerate), damage);

	}

	void Update()
	{
		if (Keyboard.current[Key.LeftArrow].wasPressedThisFrame) Navigate(false);
		if (Keyboard.current[Key.RightArrow].wasPressedThisFrame) Navigate(true);
	}
}
