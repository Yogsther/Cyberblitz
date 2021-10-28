using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTerminatedScreen : MonoBehaviour
{

	public GameObject screen;
	public Text reason;

	void Start()
	{
		ClientConnection.On("GAME_TERMINATED", packet =>
		{
			screen.SetActive(true);
			reason.text = packet.content;
		});
	}
}
