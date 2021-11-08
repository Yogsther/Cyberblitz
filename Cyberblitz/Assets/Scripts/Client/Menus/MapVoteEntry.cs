using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapVoteEntry : MonoBehaviour
{
	public Image mapImage, outline;
	public TMP_Text mapTitle, upvotes, downvotes;
	public Button upvoteButton, downvoteButton;

	public Color32 upvoteColor, downvoteColor;

	public void Setup(LevelData data)
	{
		mapTitle.text = data.name;
		upvoteButton.onClick.AddListener(() =>
		{
			Debug.Log("Voted on " + data);
		});
	}
}
