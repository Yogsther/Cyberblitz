using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
		foreach (UnitType type in UnitDataManager.GetUnitTypes())
		{
			if (type != UnitType.Courier)
			{
				Button addUnitButton = Instantiate(addUnitButtonPrefab, addUnitButtons).GetComponent<Button>();
				addUnitButton.GetComponentInChildren<TMP_Text>().text = type.ToString().ToUpper();
				addUnitButton.onClick.AddListener(() =>
				{
					openPanelButtonText.text = "CHANGE";
					playPage.SelectUnit(type, index);
					ClosePanel();
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
