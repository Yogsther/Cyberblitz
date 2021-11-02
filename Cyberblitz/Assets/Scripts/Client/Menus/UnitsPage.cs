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

	public TMP_Text title, description, story;
	public Sprite dotFilled, dotEmpty;

	public Transform health, speed, damage;

	GameObject unitImageDisplay = null;

	void Start()
	{
		unitTypeList = new List<UnitType>(UnitDataManager.unitDataDict.Keys);
		if (unitTypeList.Count == 0) Debug.LogError("Unit list is empty!");
		else selectedUnitType = unitTypeList[selectedUnitIndex];
		LoadUnit();
	}

	void Awake()
	{
		MenuSystem.OnScreenLoaded += name =>
		{
			if (name == "units") LoadUnit();
		};
	}

	void SetDots(float value, Transform dots)
	{
		for (int i = 0; i < dots.childCount; i++)
			dots.GetChild(i).GetComponent<Image>().sprite = value >= i ? dotFilled : dotEmpty;
	}

	public void Navigate(bool forward)
	{
		selectedUnitIndex = (selectedUnitIndex + (forward ? 1 : -1)) % unitTypeList.Count;
		if (selectedUnitIndex == -1) selectedUnitIndex = unitTypeList.Count - 1;

		LoadUnit();
	}

	void LoadUnit()
	{
		Debug.Log(selectedUnitIndex + " , index.");
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
		SetDots(selectedUnitData.stats.damage, damage);

	}

	void Update()
	{

		if (Keyboard.current[Key.LeftArrow].wasPressedThisFrame) Navigate(false);
		if (Keyboard.current[Key.RightArrow].wasPressedThisFrame) Navigate(true);
	}
}
