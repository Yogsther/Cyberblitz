using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
	public static Dictionary<string, Timer> activeTimers = new Dictionary<string, Timer>();


	public string StartTimer(float duration, Action OnFinished)
	{
		Timer timer = new Timer(duration, OnFinished);

		StartCoroutine(timer.Coroutine);

		activeTimers.Add(timer.id, timer);

		return timer.id;
	}

	public void StopTimer(string timerId)
	{
		Timer timer = activeTimers[timerId];

		StopCoroutine(timer.Coroutine);

		activeTimers.Remove(timerId);
	}

	public static IEnumerator TimerCoroutine(Timer timer)
	{
		yield return new WaitForSeconds(timer.duration);
		timer.OnFinished.Invoke();
	}
}

public class Timer
{
	public string id;
	public float duration;
	public Action OnFinished;
	public IEnumerator Coroutine;

	public Timer(float duration, Action OnFinished)
	{
		id = TimerID.New;
		this.duration = duration;
		this.OnFinished = OnFinished;

		Coroutine = TimerManager.TimerCoroutine(this);
	}
}