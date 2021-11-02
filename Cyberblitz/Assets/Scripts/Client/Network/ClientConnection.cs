using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System;
using Newtonsoft.Json;

public class ClientConnection
{

	public Config config;

	static WebSocket ws;
	public static NetworkEvents events = new NetworkEvents();

	public List<NetworkPacket> callstack = new List<NetworkPacket>();

	public static Action OnConnected;
	public static string version;


	public static bool IsConnected()
	{
		// DONT USE ws.IsAline, it sends a ping to the server and waits for it.
		return ws.ReadyState == WebSocketState.Open;
	}

	public void Disconnect()
	{
		ws.Close();
	}

	public ClientConnection(Config config, bool live)
	{
		this.config = config;
		version = config.version;
		ws = new WebSocket(live ? "ws://stable.cyberblitz.okdev.se:5009/" : "ws://localhost:5009");
		/*ws = new WebSocket(live ? "wss://stable.cyberblitz.okdev.se/" : "ws://localhost");*/
		/*ws.SetProxy("https://stable.cyberblitz.okdev.se", null, null);*/

		ws.OnMessage += (sender, e) =>
		{
			NetworkPacket packet = NetworkPacket.fromJSON(e.Data);
			callstack.Add(packet);
		};

		ws.OnClose += (sender, e) =>
		{
			Debug.LogWarning("Disconnected!");
		};

		On("CONNECTED", packet =>
		{
			Debug.Log($"Connected!");
			OnConnected?.Invoke();
			ClientLogin.Login();
		});

		ws.OnError += (sender, e) =>
		{
			Debug.Log(e.Message);
		};

		// Should be last in constructor
		ws.Connect();
	}

	public static void Emit(string identifier, object content)
	{
		NetworkPacket packet = new NetworkPacket(version, ClientLogin.token, identifier, content);
		ws.Send(packet.ToJSON());
	}

	public static void Emit(string identifier)
	{
		Emit(identifier, "");
	}

	public static void On(string identifier, Action<NetworkPacket> callback)
	{
		events.On(identifier, callback);
	}


}

