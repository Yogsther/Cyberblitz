using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class IntroPage : MonoBehaviour
{
	public TMP_Text introStatusText;
	public bool ready = false;
	public MenuSystem menuSystem;

	void Start()
	{
		UpdateState();
		ClientConnection.OnConnected += () =>
		{
			UpdateState();
		};
	}

	void UpdateState()
	{
		ready = ClientConnection.IsConnected();
		introStatusText.text = ready ? "[ PRESS ANY BUTTON TO CONTINUE ]" : "[ CONNECTING, PLEASE WAIT ]";
	}

	void Update()
	{
		if (ready && Keyboard.current.anyKey.wasPressedThisFrame)
		{
			menuSystem.LoadScreen("login");
			menuSystem.header.SetActive(false);
			menuSystem.menuBackground.SetActive(false);
		}

	}
}
