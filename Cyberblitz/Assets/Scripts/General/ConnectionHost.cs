using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Loads Scripts/config.json and stores them in it self.
/// </summary>
public class ConnectionHost : MonoBehaviour
{
	public enum ConnectionType
	{
		Server, Client
	}

	public ConnectionType connectionType;

	Config config;
	ServerConnection serverConnection;


	private void Awake()
	{
		// Load config.json and parse it to <Config>
		config = JsonConvert.DeserializeObject<Config>(json.ToString());

		// Start hosting server
		if (connectionType == ConnectionType.Server)
		{
			serverConnection = new ServerConnection(config);
		}

	}

	public TextAsset json;
}
