using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeFormat : MonoBehaviour
{
	public static string TimeToString(float seconds)
	{
		string result = (Mathf.Floor(seconds * 10) / 10).ToString();
		if (result.IndexOf('.') == -1) result += ".0";
		return result;
	}
}
