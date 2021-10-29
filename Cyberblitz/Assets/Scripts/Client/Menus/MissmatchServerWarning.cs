using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissmatchServerWarning : MonoBehaviour
{
	public GameObject warning;
	public Text details;
	void Start()
	{
		ClientConnection.On("MISSMATCH_VERSION", packet =>
		{
			details.text = $"SERVER {packet.content} ~ CLIENT {ClientConnection.version}";
			warning.SetActive(true);
		});
	}
}
