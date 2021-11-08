using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static PlayPage;

public class AddUnitPanel : MonoBehaviour
{

	public int index;
	public GameObject panel;
	public TMP_Text panelTitle, openPanelButtonText;
	public Transform addUnitButtons;
	public Button openPanelButton;
	[HideInInspector]
	public PlayPage playPage;
	public GameObject addUnitButtonPrefab;

	public Color nonInteractableColor;


	public void ClosePanel()
	{
		panel.SetActive(false);
	}


	public void OpenPanel()
	{
		panel.SetActive(true);
		foreach (Transform child in addUnitButtons)
		{
			Destroy(child.gameObject);
		}

		SelectedUnit[] selectedUnits = PlayPage.selectedUnits;

		foreach (UnitType type in UnitDataManager.GetUnitTypes())
		{
			if (type != UnitType.Courier)
			{
				Button addUnitButton = Instantiate(addUnitButtonPrefab, addUnitButtons).GetComponent<Button>();

				bool canBeSelected = true;
				foreach (SelectedUnit selectedUnit in selectedUnits) if (selectedUnit != null && !selectedUnit.empty && selectedUnit.type == type) canBeSelected = false;
				addUnitButton.interactable = canBeSelected;

				TMP_Text buttonText = addUnitButton.GetComponentInChildren<TMP_Text>();
				buttonText.text = type.ToString().ToUpper();
				if (!canBeSelected) buttonText.faceColor = nonInteractableColor;


				addUnitButton.onClick.AddListener(() =>
				{
					if (canBeSelected)
					{
						openPanelButtonText.text = "CHANGE";
						playPage.SelectUnit(type, index);
						ClosePanel();
					}
				});


			}

		}

	}

	void Start()
	{

		openPanelButton.onClick.AddListener(() =>
		{
			if (!panel.activeSelf) playPage.OpenPanel(this);
			else ClosePanel();
		});
	}

	void Update()
	{

	}
}
