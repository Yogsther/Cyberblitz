using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ChallengeEntry : MonoBehaviour
{

	[System.Serializable]
	public class UserStateVisuals
	{
		public UserState state;
		public Sprite icon;
		public Color32 iconColor, buttonColor;
		public string unavalibleText;
		public bool canBeChallenged;
	}

	public TMP_Text username, buttonText;
	public Image icon, outline, buttonImage;
	public Button button;

	public Color32 acceptColor, buttonTextDefault, buttonTextSent;

	public UserStateVisuals[] stateVisuals;

	public User user;

	public ChallengeManager cm;
	public InviteStatus inviteStatus;

	public void UpdateInfo(User user, InviteStatus inviteStatus)
	{
		this.user = user;
		this.inviteStatus = inviteStatus;


		UserStateVisuals visuals = GetStateVisuals(user.state);

		Color32 buttonColor = inviteStatus == InviteStatus.hasBeenInvited ? acceptColor : visuals.buttonColor;

		outline.color = buttonColor;
		buttonImage.color = buttonColor;

		icon.sprite = visuals.icon;
		icon.color = visuals.iconColor;

		string inviteText = "Challenge";
		if (inviteStatus == InviteStatus.hasInvited)
		{
			inviteText = "Sent";
			buttonImage.color = new Color32(0, 0, 0, 0);
		}

		buttonText.color = inviteStatus == InviteStatus.hasInvited ? buttonTextSent : buttonTextDefault;

		if (inviteStatus == InviteStatus.hasBeenInvited) inviteText = "Accept";

		if (!visuals.canBeChallenged) inviteText = visuals.unavalibleText;

		button.interactable = visuals.canBeChallenged;

		buttonText.text = inviteText;
	}

	public void Setup(User user)
	{
		username.text = user.username;
		button.onClick.AddListener(() =>
		{
			cm.Invite(user.id);
		});
	}

	UserStateVisuals GetStateVisuals(UserState state)
	{
		foreach (UserStateVisuals visuals in stateVisuals)
		{
			if (visuals.state == state) return visuals;
		}
		return null;
	}
}
