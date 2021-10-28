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
		User dummy = new User();
		dummy.username = $"User #{UnityEngine.Random.Range(0, 1000)}";

		Debug.Log($"Creating and loggin in dummy user {dummy.username}");

		ServerCore.ConnectUser(dummy, Token.New, packet.socket);
	}
}
