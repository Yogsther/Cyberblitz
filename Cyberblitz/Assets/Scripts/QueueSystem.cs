using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueSystem : MonoBehaviour
{
	public static Dictionary<string, List<Action>> events = new Dictionary<string, List<Action>>();
	public static List<string> calls = new List<string>();

	public static void Subscribe(string name, Action callback)
	{
		if (!events.ContainsKey(name)) events[name] = new List<Action>();
		events[name].Add(callback);
	}

	public static void Call(string name)
	{
		if (events.ContainsKey(name))
			calls.Add(name);

	}

	void Update()
	{
		if (calls.Count > 0)
		{
			foreach (string call in calls)
				foreach (Action action in events[call])
					action.Invoke();

			calls.Clear();
		}
	}
}
