using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChallengeManager : MonoBehaviour
{

	public Transform entriesTransform;
	public ChallengeEntry entryPrefab;
	public List<ChallengeEntry> entries = new List<ChallengeEntry>();
	public GameObject warning;
	public GameObject noPlayersOnline;
	public GameObject matchmakingBanner;
	public Action<User> OnLocalUserUpdate;
	public TMP_Text matchmakingButtonText;

	UserList userList;

	bool inMatchMaking = false;

	void Start()
	{
		ClearList();
		ClientConnection.On("USER_LIST", OnUserList);
		ClientConnection.On("INVITED", packet =>
		{
			SoundManager.PlaySound("invite_received");
		});
		PlayPage.OnUnitsAssembled += () => { warning.SetActive(false); };
		warning.SetActive(true);
		OnLocalUserUpdate += OnUserUpdate;
		matchmakingBanner.SetActive(false);
	}


	public void PlayBot()
	{
		if (PlayPage.HasSelectedUnits())
			ClientConnection.Emit("PLAY_BOT", new PlayRequest(PlayPage.GetSelectedUnits()));
	}

	void OnUserUpdate(User user)
	{
		ClientLogin.user.state = user.state;
		inMatchMaking = user.state == UserState.InPool;
		UpdateMatchmakingVisuals();
	}

	public void ToggleMatchmaking()
	{
		if (PlayPage.HasSelectedUnits())
		{
			inMatchMaking = !inMatchMaking;
			SoundManager.PlaySound(inMatchMaking ? "enter_matchmaking" : "leave_matchmaking");
			ClientConnection.Emit("TOGGLE_MATCHMAKING");
		} else
		{
			SoundManager.PlaySound("error");
		}
		UpdateMatchmakingVisuals();
	}

	public void UpdateMatchmakingVisuals()
	{
		matchmakingBanner.SetActive(inMatchMaking);
		matchmakingButtonText.text = inMatchMaking ? "STOP MATCHMAKING" : "START MATCHMAKING";
	}

	void UpdateEntries()
	{
		List<UserID> onlineUsers = new List<UserID>();
		// Update user statuses and invites
		foreach (User user in userList.users)
		{
			// Update local state
			if (user.id == ClientLogin.user.id) OnLocalUserUpdate?.Invoke(user);
			else
			{
				onlineUsers.Add(user.id);
				// Add or update entry
				ChallengeEntry entry = GetEntry(user);
				if (entry == null) entry = CreateEntry(user);

				entry.UpdateInfo(user, userList.invites.GetInviteStatus(ClientLogin.user.id, user.id));
			}
		}

		noPlayersOnline.SetActive(onlineUsers.Count == 0);

		// Remove offline users
		foreach (ChallengeEntry entry in entries.ToArray())
		{
			if (!onlineUsers.Contains(entry.user.id))
			{
				entries.Remove(entry);
				Destroy(entry.gameObject);
			}
		}

		// Sort entries (not here, but when updating invites)
	}

	void OnUserList(NetworkPacket packet)
	{
		userList = packet.Parse<UserList>();
		UpdateEntries();
	}

	ChallengeEntry GetEntry(User user)
	{
		foreach (ChallengeEntry entry in entries)
		{
			if (entry.user.id == user.id) return entry;
		}
		return null;
	}

	public void Invite(UserID user)
	{
		userList.invites.CreateInvite(ClientLogin.user.id, user);
		ClientConnection.Emit("INVITE", user);
		SoundManager.PlaySound("invite_sent");
		UpdateEntries();
	}

	public ChallengeEntry CreateEntry(User user)
	{
		ChallengeEntry entry = Instantiate(entryPrefab, entriesTransform);
		entry.Setup(user);
		entries.Add(entry);
		entry.cm = this;
		return entry;
	}

	void ClearList()
	{
		entries.Clear();
		foreach (Transform trans in entriesTransform) Destroy(trans.gameObject);
	}

}
