using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CyberBlitzSettings
{
	public bool fullscreen = false;
	public int width = 1280;
	public int height = 720;
	public int hz = 60;
	public float music = 1f, effects = 1f, ambience = 0.15f;
}

public static class Settings
{
	public static CyberBlitzSettings settings = new CyberBlitzSettings();

	public static Action OnResolutionChanged;
	public static Action OnAudioChanged;
	static string path = Application.persistentDataPath + "/settings.cyber";

	public static void LoadSettings()
	{
		if (File.Exists(path))
		{
			settings = JsonConvert.DeserializeObject<CyberBlitzSettings>(File.ReadAllText(path));
		}

		OnResolutionChanged?.Invoke();
		OnAudioChanged?.Invoke();
	}

	public static void SaveSettings()
	{
		OnAudioChanged?.Invoke();
		File.WriteAllText(path, JsonConvert.SerializeObject(settings));
	}

	public static void SetResolution(int width, int height, int hz)
	{
		settings.hz = hz;
		settings.width = width;
		settings.height = height;
		OnResolutionChanged?.Invoke();
		SaveSettings();
	}

	public static void SetFullscreen(bool fullscreen)
	{
		settings.fullscreen = fullscreen;
		OnResolutionChanged?.Invoke();
		SaveSettings();
	}

}
