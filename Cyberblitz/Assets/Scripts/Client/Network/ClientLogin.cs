using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ClientLogin
{
	public static Token token = "NOT_LOGGED_IN_YET";
	public static User user;

	public static Action<User> OnLogin;

	public static bool loggedIn = false;
	static bool initiated = false;
	static void Init()
	{
		// This init can only be run once.
		if (initiated) return;
		initiated = true;

		// Listen to all events it needs
		ClientConnection.On("logged_in", OnLoginConfirmed);
	}

	// Test login function, not comeplete
	public static void Login()
	{
		Init();
		ClientConnection.Emit("login");
	}

	/// <summary>
	/// The Login system gets the login credentials back from the server and is now logged in.
	/// </summary>
	static void OnLoginConfirmed(NetworkPacket packet)

	{
		ConnectedUser connectedUser = packet.Parse<ConnectedUser>();

		token = connectedUser.token;
		user = connectedUser.user;

		Debug.Log($"Logged in as {user.username}!");
		loggedIn = true;

		OnLogin?.Invoke(user);
	}

	public static bool IsLoggedIn()
	{
		return user != null && ClientConnection.IsConnected();
	}
}
