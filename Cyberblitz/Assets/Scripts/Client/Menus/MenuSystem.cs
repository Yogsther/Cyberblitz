using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MenuScreen
{
	public string name;
	public GameObject screen;
}
public class MenuSystem : MonoBehaviour
{

	[HideInInspector]
	MenuScreen selectedMenuScreen = null;
	public MenuScreen[] menuScreens;

	public static Action<string> OnScreenLoaded;

	public Button testButton;

	public GameObject mainMenu;

	public Dictionary<string, Action> OnPageLoad = new Dictionary<string, Action>();

	public MenuScreen GetScreen(string name)
	{
		foreach (MenuScreen screen in menuScreens)
		{
			if (screen.name == name) return screen;
		}
		Debug.LogError("Screen not found: " + name);
		return null;
	}

	public void LoadScreen(string name)
	{

		if (selectedMenuScreen != null) selectedMenuScreen.screen.SetActive(false);
		selectedMenuScreen = GetScreen(name);
		selectedMenuScreen.screen.SetActive(true);
		if (OnPageLoad.ContainsKey(name)) OnPageLoad[name]();


		OnScreenLoaded?.Invoke(name);
	}

	void Awake()
	{
		// Example of how to use OnPageLoad
		/*OnPageLoad["units"] = OnUnitPage;*/
	}


	public void SetMainMenuVisibility(bool visible)
	{
		mainMenu.SetActive(visible);
	}

	void Update()
	{

	}
}