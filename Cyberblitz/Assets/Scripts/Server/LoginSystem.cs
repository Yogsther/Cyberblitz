using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LoginSystem
{

	public static void Init()
	{
		ServerConnection.On("login", LoginUser);
	}

	public static void LoginUser(NetworkPacket packet)
	{
		User user = new User();
		user.username = packet.content;

		Debug.Log($"User connected: {user.username}");

		ServerCore.ConnectUser(user, Token.New, packet.socket);
	}
}
