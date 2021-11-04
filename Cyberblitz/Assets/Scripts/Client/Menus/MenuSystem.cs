using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
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
	public GameObject menuBackground;

	public Camera lobbyCamera, planningCamera;
	public GameObject lobbyWorld;

	[HideInInspector]
	MenuScreen selectedMenuScreen = null;
	public MenuScreen[] menuScreens;

	public static Action<string> OnScreenLoaded;

	public Button testButton;

	public GameObject mainMenu;

	public Transform subHeader;
	public GameObject subHeaderButton;

	public Dictionary<string, Action> OnPageLoad = new Dictionary<string, Action>();

	public CustomPassVolume outlineVolume;

	public void SetOutlineVisibility(bool enabled)
	{
		outlineVolume.enabled = enabled;
	}

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
			lobbyCamera.enabled = false;
			SetOutlineVisibility(true);
			lobbyWorld.SetActive(false);
			Debug.Log("Test");
		};
	}

	public void DisplayGameUI(bool show)
	{
		gameUI.SetActive(show);
	}

	public void LoadScreen(string name)
	{
		lobbyWorld.SetActive(true);
		planningCamera.enabled = false;
		lobbyCamera.enabled = true;
		SetOutlineVisibility(false);

		if (!menuBackground.activeSelf) menuBackground.SetActive(true);

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
