using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
	public Image victoryText, defeatText;
	public Button returnButton;
	public TMP_Text postGameText;
	public GameObject gameOverScreen;
	public MatchManager matchManager;
	public CinematicCamera cinematicCamera;

	private void Awake()
	{
		returnButton.onClick.AddListener(() =>
		{
			matchManager.UnloadMatch();
		});


		MatchManager.OnMatchEnd += (Match match, string reason) =>
		{
			bool won = true;
			Debug.Log("Got game end screen");
			if (match.winner != null) won = match.winner == ClientLogin.user.id;

			DisplayPostGame(won, reason);
			/*SoundManager.PlaySound("game_over");*/
			cinematicCamera.StartCircling();
		};
	}

	public void DisplayPostGame(bool victory, string reason)
	{
		gameOverScreen.SetActive(true);

		victoryText.enabled = victory;
		defeatText.enabled = !victory;
		postGameText.text = reason;
	}

	public void HideScreen()
	{
		gameOverScreen.SetActive(false);
	}







}
