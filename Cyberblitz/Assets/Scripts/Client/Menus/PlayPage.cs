using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayPage : MonoBehaviour
{
	public MenuSystem menuSystem;
	public AddUnitPanel[] panels;

	private void Awake()
	{
		menuSystem.OnPageLoad["play"] = OnPageLoad;
	}

	void OnPageLoad()
	{
		foreach (AddUnitPanel panel in panels)
		{
			panel.playPage = this;
			panel.ClosePanel();
		}
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
