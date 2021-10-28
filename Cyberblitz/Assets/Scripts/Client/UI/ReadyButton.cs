using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadyButton : MonoBehaviour
{

	public float timer = 0f;
	public Button readyButton;
	public Text stateText, timerText;

	void Start()
	{
		MatchManager.OnMatchUpdate += OnMatchUpdate;
		readyButton.onClick.AddListener(() => ReadyUp());
	}

	void OnMatchUpdate(Match match)
	{
		timerText.text = "--:--";
		if (match.state == Match.GameState.Planning)
		{
			timer = match.rules.planningTime;
		}
		readyButton.interactable = match.state == Match.GameState.Planning;
		stateText.text = match.state.ToString().ToUpper();
	}

	void ReadyUp()
	{
		if (MatchManager.match == null) return;

		if (MatchManager.match.state == Match.GameState.Planning)
		{
			stateText.text = "READY";
			readyButton.interactable = false;
			MatchManager.SignalReady();
		}
	}

	private void Update()
	{
		if (MatchManager.match != null)
		{
			if (MatchManager.match.state == Match.GameState.Planning)
			{
				timer -= Time.deltaTime;
				if (timer < 0) timer = 0;
				timerText.text = TimeFormat.TimeToString(timer) + "S";
			}
		}
	}

}
