using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapVoteEntry : MonoBehaviour
{
	public MapVotePage mapVotePage;
	public Image mapImage, outline;
	public TMP_Text mapTitle, upvotes, downvotes;
	public Button upvoteButton, downvoteButton;

	public Color32 upvoteColor, downvoteColor, defaultColor, undecidedColor;
	public Image upvoteImage, downvoteImage;

	public LevelData levelData;

	public void Setup(LevelData data)
	{
		levelData = data;
		mapImage.sprite = levelData.thumbnail;
		mapTitle.text = data.name;
		upvoteButton.onClick.AddListener(() =>
		{
			mapVotePage.Vote(VoteType.Upvote, data.id);
		});

		downvoteButton.onClick.AddListener(() =>
		{
			mapVotePage.Vote(VoteType.Downvote, data.id);
		});
	}

	public void UpdateState(VoteType type, int up, int down)
	{
		upvoteImage.color = (type == VoteType.Upvote) ? upvoteColor : defaultColor;
		downvoteImage.color = (type == VoteType.Downvote) ? downvoteColor : defaultColor;

		if (type == VoteType.Undecided) outline.color = undecidedColor;
		else outline.color = (type == VoteType.Upvote) ? upvoteColor : downvoteColor;


		upvotes.text = up.ToString();
		downvotes.text = down.ToString();

	}
}
