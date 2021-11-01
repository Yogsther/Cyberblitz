using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

/// <summary>
/// Creates a new Websocket host for the server.
/// WebSocketSharp documentation https://github.com/sta/websocket-sharp
/// </summary>


public class SocketInstance : WebSocketBehavior
{
	ServerConnection parent;
	protected override void OnMessage(MessageEventArgs e)
	{
		parent.OnMessage(e.Data, ID);
	}

	protected override void OnClose(CloseEventArgs e)
	{
		parent.OnDisconnect(ID);
	}

	protected override void OnOpen()
	{
		this.parent.OnConnection(ID);
	}

	public SocketInstance(ServerConnection parent)
	{
		this.parent = parent;
	}
}

public class ServerConnection : WebSocketBehavior
{

	static WebSocketServer wssv;

	static NetworkEvents events = new NetworkEvents();
	Config config;

	public void OnMessage(string message, SocketID socket)
	{
		NetworkPacket packet = NetworkPacket.fromJSON(message);
		packet.socket = socket;

		if (packet.version != config.version)
		{
			SendTo(socket, "MISSMATCH_VERSION", config.version);
			return;
		}

		ConnectedUser user = ServerCore.GetConnectedUser(packet.token);
		if (user != null)
			packet.user = user.user;

		events.Invoke(packet);
	}

	public void OnConnection(SocketID socket)
	{
		SendTo(socket, "CONNECTED");
	}

	public void OnDisconnect(SocketID socket)
	{
		ServerCore.DisconnectUser(socket);
	}

	public static void SendTo(SocketID socket, string message)
	{
		SendTo(socket, message, "");
	}

	public static void SendTo(SocketID id, string message, object content)
	{
		wssv.WebSocketServices["/"].Sessions.SendTo(new NetworkPacket(message, content).ToJSON(), id);
	}

	public ServerConnection(Config config)
	{
		this.config = config;
		wssv = new WebSocketServer("ws://localhost");
		wssv.AddWebSocketService("/", () => new SocketInstance(this));

		// Should be be last in this constructor
		wssv.Start();
		Debug.Log($"----------------------------------\n\n\n" +
			$"Started CyberBlitz server {config.version} on port {config.port}");
	}

	public static void On(string identifier, Action<NetworkPacket> callback)
	{
		events.On(identifier, callback);
	}

	public static void OnMatchContext(MatchID id, string identifier, Action<NetworkPacket> callback)
	{
		events.OnMatchContext(id, identifier, callback);
	}
}
