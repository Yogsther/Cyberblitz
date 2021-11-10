using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeManager : MonoBehaviour
{

	public Transform entriesTransform;
	public ChallengeEntry entryPrefab;
	public List<ChallengeEntry> entries = new List<ChallengeEntry>();
	public GameObject warning;

	UserList userList;

	void Start()
	{
		ClearList();
		ClientConnection.On("USER_LIST", OnUserList);
		PlayPage.OnUnitsAssembled += () => { warning.SetActive(false); };
	}

	public void PlayBot()
	{
		if (PlayPage.HasSelectedUnits())
			ClientConnection.Emit("PLAY_BOT", new PlayRequest(PlayPage.GetSelectedUnits()));
	}

	void UpdateEntries()
	{
		List<UserID> onlineUsers = new List<UserID>();
		// Update user statuses and invites
		foreach (User user in userList.users)
		{
			onlineUsers.Add(user.id);
			// Update local state
			if (user.id == ClientLogin.user.id) ClientLogin.user.state = user.state;
			else
			{
				// Add or update entry
				ChallengeEntry entry = GetEntry(user);
				if (entry == null) entry = CreateEntry(user);

				entry.UpdateInfo(user, userList.invites.GetInviteStatus(ClientLogin.user.id, user.id));
			}
		}

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
