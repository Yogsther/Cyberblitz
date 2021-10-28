using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBrowser : MonoBehaviour
{

	public UserListEntry userListEntryPrefab;
	public Transform userListParent;
	public Image background;
	public Text title;

	void Start()
	{
		ClientConnection.On("USER_LIST", LoadUserList);
		MatchManager.OnMatchStart += (match) => { SetVisbility(false); };
		SetVisbility(true);
	}

	void SetVisbility(bool visibility)
	{
		background.enabled = visibility;
		title.enabled = visibility;
		userListParent.gameObject.SetActive(visibility);
	}

	public void LoadUserList(NetworkPacket packet)
	{
		ClearList();
		List<User> userList = packet.Parse<List<User>>();

		LoadUser("Play against bot", () => ClientConnection.Emit("PLAY_BOT"));

		foreach (User user in userList)
			if (user.id != ClientLogin.user.id)
				LoadUser(user.username, () => ClientConnection.Emit("PLAY_PLAYER", user.id));
	}

	void ClearList()
	{
		Debug.Log("Clear list");
		foreach (Transform child in userListParent)
		{
			Destroy(child.gameObject);
		}
	}

	public void LoadUser(string username, Action callback)
	{
		UserListEntry entry = Instantiate(userListEntryPrefab, userListParent);
		entry.username.text = username;
		entry.button.onClick.AddListener(() =>
		{
			callback();
		});
	}

	void Update()
	{

	}
}
