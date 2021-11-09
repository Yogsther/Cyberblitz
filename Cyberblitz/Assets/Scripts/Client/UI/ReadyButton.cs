using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadyButton : MonoBehaviour
{

	public float timer = 0f;
	public Button readyButton;
	public Text timerText;

	void Awake()
	{
		MatchManager.OnMatchUpdate += OnMatchUpdate;
		readyButton.onClick.AddListener(() => ReadyUp());

		MatchManager.OnPlanningStart += OnPlanningStart;
		MatchManager.OnPlanningEnd += OnPlanningEnd;
	}

	void OnMatchUpdate(Match match)
	{
		timerText.text = "--";
		if (match.state == Match.GameState.Planning)
		{
			timer = match.rules.planningTime;
		}
		readyButton.interactable = match.state == Match.GameState.Planning;
	}

	void OnPlanningStart()
	{
		timerText.gameObject.SetActive(true);
	}

	void OnPlanningEnd()
	{
		timerText.gameObject.SetActive(false);
	}

	void ReadyUp()
	{
		if (MatchManager.match == null) return;

		if (MatchManager.match.state == Match.GameState.Planning)
		{
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
				if (timer > 0)
				{
					timer -= Time.deltaTime;
					if (timer <= 0)
					{
						timer = 0;
						MatchManager.SignalReady();
					}
					timerText.text = Mathf.Floor(timer).ToString();
				}

			}
		}
	}

}
