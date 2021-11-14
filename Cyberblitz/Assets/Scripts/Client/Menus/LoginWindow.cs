using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LoginWindow : MonoBehaviour
{

	public TMP_InputField usernameInput;
	public Button loginButton;
	public MenuSystem menuSystem;
	public bool sentLogin = false;
	public HowToPlayPage tutorial;

	void Start()
	{
		sentLogin = false;
		loginButton.onClick.AddListener(() =>
		{
			Login();
		});

		ClientLogin.OnLogin += user =>
		{
			menuSystem.selectedMenuScreen.screen.SetActive(false);
			menuSystem.StartLoginAnimation();
			if (Settings.firstStartup) tutorial.DisplayTutorial();
		};

	}

	void Login()
	{
		if (sentLogin) return;
		sentLogin = true;
		ClientLogin.Login(usernameInput.text);
	}

	void Update()
	{
		if (Keyboard.current[Key.Enter].wasPressedThisFrame) Login();
	}
}
