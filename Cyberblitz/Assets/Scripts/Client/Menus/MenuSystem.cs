using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
	public GameObject gameUI;

	[HideInInspector]
	MenuScreen selectedMenuScreen = null;
	public MenuScreen[] menuScreens;

	public static Action<string> OnScreenLoaded;

	public Button testButton;

	public GameObject mainMenu;

	public Transform subHeader;
	public GameObject subHeaderButton;

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

	private void Awake()
	{
		MatchManager.OnMatchStart += match =>
		{
			DisplayGameUI(true);
		};
	}

	public void DisplayGameUI(bool show)
	{
		gameUI.SetActive(show);
	}

	public void LoadScreen(string name)
	{
		ClearSubHeader();
		if (selectedMenuScreen != null) selectedMenuScreen.screen.SetActive(false);
		selectedMenuScreen = GetScreen(name);
		selectedMenuScreen.screen.SetActive(true);
		if (OnPageLoad.ContainsKey(name)) OnPageLoad[name]();

		OnScreenLoaded?.Invoke(name);
		DisplayGameUI(false);
	}

	public void ClearSubHeader()
	{
		foreach (Transform obj in subHeader)
			Destroy(obj.gameObject);
	}

	public void CreateSubHeaderButton(string title, Action func)
	{
		Button button = Instantiate(subHeaderButton, subHeader).GetComponent<Button>();
		button.onClick.AddListener(() =>
		{
			func();
		});
		button.GetComponentInChildren<TMP_Text>().text = title;
	}

	private void Start()
	{
		LoadScreen("play");
	}


	public void SetMainMenuVisibility(bool visible)
	{
		mainMenu.SetActive(visible);
	}

}
