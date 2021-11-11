using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Loads Scripts/config.json and stores them in it self.
/// </summary>
public class ConnectionConfiguration : MonoBehaviour
{
	public enum ConnectionType
	{
		Server, Client
	}

	public bool live;

	public ConnectionType connectionType;

	Config config;

	public static ServerConnection serverConnection;
	public static ClientConnection clientConnection;
	public static Action<string> OnVersionNumber;


	private void Start()
	{
		// Load config.json and parse it to <Config>
		config = JsonConvert.DeserializeObject<Config>(json.ToString());

		if (connectionType == ConnectionType.Server)
		{
			// Start hosting server
			serverConnection = new ServerConnection(config);
			ServerCore.Init();
			LoginSystem.Init();
		} else
		{
			// Connect to server
			clientConnection = new ClientConnection(config, live);
			MatchManager.Instance.Init();
			OnVersionNumber?.Invoke(config.version);
		}
	}



	private void Update()
	{
		if (connectionType == ConnectionType.Client)
		{
			foreach (NetworkPacket packet in clientConnection.callstack.ToArray())
			{
				ClientConnection.events.Invoke(packet);
			}
			clientConnection.callstack.Clear();
		}
	}

	private void OnApplicationQuit()
	{
		if (clientConnection != null)
		{
			clientConnection.Disconnect();
		}
	}

	public TextAsset json;
}
