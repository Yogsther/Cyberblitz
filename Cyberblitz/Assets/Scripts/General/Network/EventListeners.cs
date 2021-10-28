using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventListeners
{

	public Dictionary<string, List<Action<NetworkPacket>>> listeners = new Dictionary<string, List<Action<NetworkPacket>>>();

}
