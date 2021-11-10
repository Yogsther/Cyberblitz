using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginWindow : MonoBehaviour
{

	public TMP_InputField usernameInput;
	public Button loginButton;
	public MenuSystem menuSystem;

	void Start()
	{
		loginButton.onClick.AddListener(() =>
		{
			ClientLogin.Login(usernameInput.text);
		});

		ClientLogin.OnLogin += user =>
		{
			menuSystem.selectedMenuScreen.screen.SetActive(false);
			menuSystem.StartLoginAnimation();
		};

	}

	void Update()
	{

	}
}
