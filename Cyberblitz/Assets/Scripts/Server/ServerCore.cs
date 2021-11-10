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
	public static Invites invites = new Invites();
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
		}
	}

	public static void TerminateGame(MatchID id)
	{
		Debug.Log("Terminated game " + id);
		Referee game = GetGame(id);
		foreach (Player player in game.match.players)
		{
			ConnectedUser user = GetConnectedUser(player.user.id);
			if (user != null) user.user.state = UserState.Ready;
		}
		games.Remove(game);
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
		UserList userList = new UserList();

		userList.invites = invites;
		userList.users = new List<User>();

		foreach (ConnectedUser connectedUser in users)
		{
			User user = connectedUser.user;

			if (GetUserGame(user.id) != null) user.state = UserState.Ingame;
			userList.users.Add(user);
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
		Debug.Log("Disconnecting socket: " + socket);
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
		/*ServerConnection.On("PLAY_PLAYER", StartGameWithPlayers);*/
		ServerConnection.On("PLAY_BOT", StartGameWithBot);
		ServerConnection.On("USER_STATE", OnUserState);
		ServerConnection.On("INVITE", OnInvite);
	}

	static void OnInvite(NetworkPacket packet)
	{
		ConnectedUser from = GetConnectedUser(packet.user.id);
		ConnectedUser to = GetConnectedUser(packet.Parse<UserID>());
		if (from != null && to != null)
		{
			if (invites.ExistsInvite(to.user.id, from.user.id))
			{
				invites.RemoveAllInvitesRelatingToUser(from.user.id);
				invites.RemoveAllInvitesRelatingToUser(to.user.id);

				// Start game with players
				StartGameWithPlayers(from, to);

			} else if (invites.CreateInvite(from.user.id, to.user.id))
				UpdateUserList();
		}
	}

	static void OnUserState(NetworkPacket packet)
	{
		UserState state = packet.Parse<UserState>();
		ConnectedUser user = GetConnectedUser(packet.user.id);
		if (user != null)
		{
			user.user.state = state;
			UpdateUserList();
		}
	}

	static void StartGameWithPlayers(ConnectedUser user1, ConnectedUser user2)
	{
		/*PlayRequest playRequest = packet.Parse<PlayRequest>();*/

		if (user1 != null && user2 != null)
		{
			Referee referee = new Referee();
			games.Add(referee);

			referee.Init();
			referee.AddPlayer(user1.user);
			referee.AddPlayer(user2.user);
			referee.SendGameUpdate();

			UpdateUserList();
		}
	}

	static void StartGameWithBot(NetworkPacket packet)
	{
		PlayRequest playRequest = packet.Parse<PlayRequest>();
		if (packet.user == null)
		{
			Debug.Log("WARNING USER NOT LOGGED IN TRIED TO START GAME!");
			return;
		}

		Referee referee = new Referee();
		games.Add(referee);

		referee.Init();
		referee.AddPlayer(packet.user);
		referee.AddBot();
		referee.SendGameUpdate();

		UpdateUserList();
	}
}
