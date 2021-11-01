using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitsPage : MonoBehaviour
{
	List<UnitType> unitTypeList;
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
		else selectedUnitType = unitTypeList[0];
		LoadUnit();
	}

	void Awake()
	{
		MenuSystem.OnScreenLoaded += name =>
		{
			if (name == "units") LoadUnit();
		};
	}

	void LoadUnit()
	{
		Debug.Log("Loading unit");
		selectedUnitData = UnitDataManager.GetUnitDataByType(selectedUnitType);
		if (unitImageDisplay) DestroyImmediate(unitImageDisplay);
		unitImageDisplay = Instantiate(selectedUnitData.image, transform);
		unitImageDisplay.gameObject.SetActive(true);

		title.text = selectedUnitData.name;
		description.text = "...?";
		story.text = selectedUnitData.description;

		Debug.Log("Loaded image.");
	}

	void Update()
	{

	}
}
