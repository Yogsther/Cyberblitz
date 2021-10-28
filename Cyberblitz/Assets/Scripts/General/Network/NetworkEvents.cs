using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkEvents
{

	EventListeners events = new EventListeners();
	Dictionary<MatchID, EventListeners> contextEvents = new Dictionary<MatchID, EventListeners>();

	public void OnMatchContext(MatchID id, string identifier, Action<NetworkPacket> callback)
	{
		if (!contextEvents.ContainsKey(id)) contextEvents.Add(id, new EventListeners());
		if (!contextEvents[id].listeners.ContainsKey(identifier)) contextEvents[id].listeners.Add(identifier, new List<Action<NetworkPacket>>());
		contextEvents[id].listeners[identifier].Add(callback);
	}

	public void On(string identifier, Action<NetworkPacket> callback)
	{
		if (!events.listeners.ContainsKey(identifier)) events.listeners.Add(identifier, new List<Action<NetworkPacket>>());
		events.listeners[identifier].Add(callback);
	}

	public void Invoke(NetworkPacket packet)
	{
		if (events.listeners.ContainsKey(packet.identifier))
			foreach (Action<NetworkPacket> func in events.listeners[packet.identifier])
				func.Invoke(packet);

		if (packet.user != null)
		{
			foreach (Referee referee in ServerCore.games)
			{
				if (referee.match.ContainsPlayer(packet.user.id))
				{
					MatchID match = referee.match.id;
					if (contextEvents.ContainsKey(referee.match.id))
					{
						if (contextEvents[match].listeners.ContainsKey(packet.identifier))
						{
							foreach (Action<NetworkPacket> func in contextEvents[match].listeners[packet.identifier])
							{
								func(packet);
							}
						}
					}
				}
			}
		}
	}
}
