using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectedUser
{
	public Token token;
	public User user;
	public SocketID socket;
	public bool connected = true;

	public void Emit(string message, object content)
	{
		if (connected) ServerConnection.SendTo(socket, message, content);
	}

	public void Emit(string message)
	{
		Emit(message, "");
	}
}
