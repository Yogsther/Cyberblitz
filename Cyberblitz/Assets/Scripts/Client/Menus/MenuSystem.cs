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
	public AudioClip music, ambience;

	public GameObject gameUI;
	public GameObject menuBackground;
	public GameObject connectingScreen;

	public LobbyCamera lobbyCamera;
	public Camera planningCamera;
	public GameObject lobbyWorld;

	public GameOverScreen gameOverScreen;

	string currentlyLoadingScreen = null;

	[HideInInspector]
	public MenuScreen selectedMenuScreen = null;
	public MenuScreen[] menuScreens;

	public static Action<string> OnScreenLoaded;

	public Button testButton;

	public GameObject mainMenu;
	public GameObject header;

	public Transform subHeader;
	public GameObject subHeaderButton;

	public TMP_Text versionNumber;

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
		selectedMenuScreen = null;

		MatchManager.OnMatchStart += match =>
		{
			DisplayGameUI(true);
			lobbyCamera.lobbyCamera.enabled = false;
			SetOutlineVisibility(true);
			lobbyWorld.SetActive(false);
		};

		MatchManager.OnMapVote += (match) =>
		{
			header.SetActive(false);
		};

		ClientConnection.OnConnected += () => { connectingScreen.SetActive(false); };
		ClientConnection.OnDisconnected += () =>
		{
			connectingScreen.SetActive(true);
			/*LoadScreen("play");*/
		};

		MatchManager.OnMatchUnloaded += () =>
		{
			LoadScreen("play");
			SoundManager.PlayMusic(music);
			SoundManager.PlayAmbience(ambience);
		};

		ConnectionConfiguration.OnVersionNumber += version =>
		{
			versionNumber.text = version;
		};
	}



	public void DisplayGameUI(bool show)
	{
		gameUI.SetActive(show);
	}

	public void LoadScreen(string name, Action callback)
	{
		if (currentlyLoadingScreen != null) return;

		if (selectedMenuScreen == null || name != selectedMenuScreen.name)
		{
			if (selectedMenuScreen != null && selectedMenuScreen.name == "play")
			{
				currentlyLoadingScreen = name;
				lobbyCamera.AnimateOut(() =>
				{
					LoadScreenElements(name);
					callback();
				});
			} else if (name == "play")
			{
				LoadScreenElements(name);
				currentlyLoadingScreen = name;
				lobbyCamera.AnimateIn(() =>
				{
					currentlyLoadingScreen = null;
					callback();
				});
			} else
			{
				LoadScreenElements(name);
				callback();
			}
		}
	}

	public void LoadScreen(string name)
	{
		LoadScreen(name, () => { });
	}


	void LoadScreenElements(string name)
	{
		header.SetActive(true);
		gameOverScreen.HideScreen();
		DisplayGameUI(false);
		SetMainMenuVisibility(true);

		currentlyLoadingScreen = null;
		lobbyWorld.SetActive(true);
		planningCamera.enabled = false;
		lobbyCamera.lobbyCamera.enabled = true;
		SetOutlineVisibility(false);

		if (!menuBackground.activeSelf) menuBackground.SetActive(true);

		ClearSubHeader();
		if (selectedMenuScreen != null && selectedMenuScreen.screen != null) selectedMenuScreen.screen.SetActive(false);
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
		foreach (MenuScreen menuScreen in menuScreens) menuScreen.screen.SetActive(false);
		LoadScreen("intro");
		SoundManager.PlayAmbience(ambience);

		menuBackground.SetActive(false);
		header.SetActive(false);
	}

	public void StartLoginAnimation()
	{
		SoundManager.PlayMusic(music);
		lobbyCamera.AnimateFromLogin(() =>
		{
			LoadScreenElements("play");
		});
	}


	public void SetMainMenuVisibility(bool visible)
	{
		mainMenu.SetActive(visible);
		lobbyWorld.SetActive(visible);
		lobbyCamera.lobbyCamera.enabled = true;
		planningCamera.enabled = false;
		SetOutlineVisibility(!visible);
	}

}
