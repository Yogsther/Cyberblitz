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
			User winner = match.GetUser(match.winner);
			if (reason == null) reason = winner.username + " won the game!";
			DisplayPostGame(winner.id == ClientLogin.user.id, reason);
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
