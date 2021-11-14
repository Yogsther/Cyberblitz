using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HowToPlayPage : MonoBehaviour
{

	public GameObject content;
	public Button returnButton;
	private void Awake()
	{
		HideTutorial();

		returnButton.onClick.AddListener(() =>
		{
			HideTutorial();
		});
	}

	public void DisplayTutorial()
	{
		content.SetActive(true);
	}

	public void HideTutorial()
	{
		content.SetActive(false);
	}
}
