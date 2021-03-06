using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System;
using Newtonsoft.Json;
using System.Threading.Tasks;

public class ClientConnection
{

	public Config config;

	public static WebSocket ws;
	public static NetworkEvents events = new NetworkEvents();

	public List<NetworkPacket> callstack = new List<NetworkPacket>();

	public static Action OnConnected;
	public static Action OnDisconnected;
	public static string version;


	public static bool IsConnected()
	{
		/*return ws.ReadyState == WebSocketState.Open;*/
		return ws.IsAlive;
	}

	public void Disconnect()
	{
		ws.Close();
	}

	public ClientConnection(Config config, bool live)
	{
		this.config = config;
		version = config.version;
		ws = new WebSocket(live ? $"ws://stable.cyberblitz.okdev.se:{config.port}" : "ws://localhost:" + config.port);
		/*ws = new WebSocket(live ? "wss://stable.cyberblitz.okdev.se/" : "ws://localhost");*/
		/*ws.SetProxy("https://stable.cyberblitz.okdev.se", null, null);*/

		ws.OnMessage += (sender, e) =>
		{
			NetworkPacket packet = NetworkPacket.fromJSON(e.Data);
			callstack.Add(packet);
		};

		ws.OnClose += (sender, e) =>
		{
			NetworkPacket disconnectMessage = new NetworkPacket("DISCONNECTED", "");
			callstack.Add(disconnectMessage);
		};

#if !UNITY_EDITOR
			Task.Run(() =>
			{
				while (!ws.IsAlive)
				{
					Task.Delay(1000).Wait();

					ws.Connect();
				}
			});
		
#endif


		On("CONNECTED", packet =>
		{
			Debug.Log($"Connected!");

			OnConnected?.Invoke();
		});

		ws.OnError += (sender, e) =>
		{
			Debug.LogError(e.Message);
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

