using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectedUser
{
	public Token token;
	public User user;
	public SocketID socket;

	public void Emit(string message, object content)
	{
		ServerConnection.SendTo(socket, message, content);
	}

	public void Emit(string message)
	{
		Emit(message, "");
	}
}
