using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPacket
{
	public string identifier;
	public string content;
	public Token token; // Token of the client sending (only used Client -> Server com)
	public SocketID socket = "NOT_ASSIGNED"; // Used by the server to store socket ids in packets.
	public User user;
	public string version;

	public NetworkPacket(string version, Token token, string identifier, object content)
	{

		this.identifier = identifier;
		this.content = content is string ? (string)content : Serialize(content);
		this.token = token;
		this.version = version;
	}

	public NetworkPacket(string identifier, object content)
	{
		this.identifier = identifier;
		this.content = content is string ? (string)content : Serialize(content);
	}

	public T Parse<T>()
	{

		JsonSerializerSettings settings = new JsonSerializerSettings
		{
			TypeNameHandling = TypeNameHandling.Objects
		};

		return JsonConvert.DeserializeObject<T>(content, settings);
	}

	public NetworkPacket() { }

	public static NetworkPacket fromJSON(string json)
	{

		JsonSerializerSettings settings = new JsonSerializerSettings
		{
			TypeNameHandling = TypeNameHandling.Objects
		};

		NetworkPacket packet = JsonConvert.DeserializeObject<NetworkPacket>(json, settings);
		return packet;
	}

	public static string Serialize(object obj)
	{
		JsonSerializerSettings settings = new JsonSerializerSettings
		{
			TypeNameHandling = TypeNameHandling.Objects
		};

		return JsonConvert.SerializeObject(obj, settings);
	}

	public string ToJSON()
	{
		return Serialize(this);
	}
}
