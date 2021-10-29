using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Keeps track of all connected users and all games going on.
/// </summary>

public static class ServerCore
{
	public static List<Referee> games = new List<Referee>();
	public static Referee GetGame(MatchID id)
	{
		foreach (Referee referee in games)
		{
			if (referee.match.id == id)
			{
				return referee;
			}
		}
		return null;
	}

	public static List<ConnectedUser> users = new List<ConnectedUser>();
	public static ConnectedUser GetConnectedUser(UserID id)
	{
		foreach (ConnectedUser user in users)
		{
			if (user.user.id == id) return user;
		}
		return null;
	}

	public static ConnectedUser GetConnectedUser(Token token)
	{
		foreach (ConnectedUser user in users)
		{
			if (user.token == token) return user;
		}
		return null;
	}

	public static void SendTo(UserID id, string message, object content)
	{
		ConnectedUser connectedUser = GetConnectedUser(id);
		if (connectedUser != null)
		{
			connectedUser.Emit(message, content);
			Debug.Log("Sent " + message + " to " + connectedUser.user.username);
		}
	}

	public static void TerminateGame(MatchID id)
	{
		games.Remove(GetGame(id));
		UpdateUserList();
	}

	public static void Broadcast(string message, object content)
	{
		foreach (ConnectedUser connectedUser in users)
		{
			SendTo(connectedUser.user.id, message, content);
		}
	}

	public static void UpdateUserList()
	{
		List<User> userList = new List<User>();
		foreach (ConnectedUser connectedUser in users)
		{
			User user = connectedUser.user;
			user.playable = GetUserGame(user.id) == null;
			userList.Add(user);
		}
		Broadcast("USER_LIST", userList);
	}

	/// <summary>
	/// Connect a user to the server, and keep them logged in until they disconnect.
	/// </summary>
	/// <param name="user">User profile, loaded from DB</param>
	/// <param name="token">User token, used to authenticate all actions by the user</param>
	/// <param name="socket">ID of the socket connection so we can identify them on disconnect and send them direct messages</param>

	public static void ConnectUser(User user, Token token, SocketID socket)
	{

		ConnectedUser connectedUser = new ConnectedUser();
		connectedUser.user = user;
		connectedUser.token = token;
		connectedUser.socket = socket;

		Debug.Log("Connected " + connectedUser.user.username);
		users.Add(connectedUser);
		ServerConnection.SendTo(connectedUser.socket, "logged_in", connectedUser);
		UpdateUserList();
	}


	public static void DisconnectUser(SocketID socket)
	{
		foreach (ConnectedUser connectedUser in users.ToList())
		{
			if (connectedUser.socket == socket)
			{
				Referee activeGame = GetUserGame(connectedUser.user.id);
				users.Remove(connectedUser);
				if (activeGame != null)
				{
					activeGame.OnPlayerDisconnect(connectedUser.user);
				}
				Debug.Log($"Disconnected user {connectedUser.user.username}\nTotal connected users: {users.Count}");
			}
		}
		UpdateUserList();
	}

	/// <summary>Get an active game that this user is in, if null the user is not in a game</summary>

	static Referee GetUserGame(UserID userId)
	{
		foreach (Referee referee in games)
			if (referee.match.GetUser(userId) != null)
				return referee;
		return null;
	}

	public static void Init()
	{
		// Listen to connection events if needed...
		ServerConnection.On("PLAY_PLAYER", StartGameWIthPlayers);
		ServerConnection.On("PLAY_BOT", StartGameWithBot);
	}

	static void StartGameWIthPlayers(NetworkPacket packet)
	{

		ConnectedUser user1 = GetConnectedUser(packet.user.id);
		ConnectedUser user2 = GetConnectedUser(packet.Parse<UserID>());

		if (user1 != null && user2 != null)
		{
			Referee referee = new Referee();
			games.Add(referee);

			referee.Init();
			referee.LoadLevel();
			referee.AddPlayer(user1.user);
			referee.AddPlayer(user2.user);
			referee.Start();

			UpdateUserList();
		}

	}

	static void StartGameWithBot(NetworkPacket packet)
	{
		if (packet.user == null)
		{
			Debug.Log("WARNING USER NOT LOGGED IN TRIED TO START GAME!");
			return;
		}

		Referee referee = new Referee();
		games.Add(referee);

		referee.Init();
		referee.LoadLevel();
		referee.AddPlayer(packet.user);
		referee.AddBot();
		referee.Start();

		UpdateUserList();
	}
}
