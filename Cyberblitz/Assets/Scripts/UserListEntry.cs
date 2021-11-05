using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserListEntry : MonoBehaviour
{

	public Text username, buttonText;
	public Button button;

	public void SetPlayable(bool playable)
	{
		button.interactable = playable;
		buttonText.text = playable ? "Play" : "Already in game";
	}

}
