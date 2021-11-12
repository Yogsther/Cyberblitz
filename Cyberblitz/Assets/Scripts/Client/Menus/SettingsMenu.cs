using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{

	public TMP_Dropdown resolutionInput;
	public Slider music, ambience, effects;
	public Toggle fullscreen;

	List<Resolution> resolutions = new List<Resolution>();
	List<string> resolutionOptions = new List<string>();

	void Awake()
	{

		// Get valid resolutions
		foreach (Resolution resolution in Screen.resolutions)
		{
			if (Mathf.Floor(((float)resolution.width / resolution.height) * 10) == 17)
			{
				resolutions.Add(resolution);
				resolutionOptions.Add($"{resolution.width}x{resolution.height}, {resolution.refreshRate}hz");
			}
		}

		resolutionInput.ClearOptions();
		resolutionInput.AddOptions(resolutionOptions);

		fullscreen.onValueChanged.AddListener(value =>
		{
			Settings.SetFullscreen(value);
		});

		ApplyAudioOption(music, Settings.settings.music, val => { Settings.settings.music = val; Settings.SaveSettings(); });
		ApplyAudioOption(ambience, Settings.settings.ambience, val => { Settings.settings.ambience = val; Settings.SaveSettings(); });
		ApplyAudioOption(effects, Settings.settings.effects, val => { Settings.settings.effects = val; Settings.SaveSettings(); });



		// Apply so settings in menu is correct to the currently loaded settings
		LoadStatesFromSettings();

		resolutionInput.onValueChanged.AddListener((int value) =>
		{
			Resolution resolution = resolutions[value];
			Settings.SetResolution(resolution.width, resolution.height, resolution.refreshRate);
		});
	}


	void ApplyAudioOption(Slider slider, float option, Action<float> onChange)
	{
		slider.value = option;

		slider.onValueChanged.AddListener((float value) =>
		{
			onChange(value);
		});
	}

	void LoadStatesFromSettings()
	{
		fullscreen.isOn = Settings.settings.fullscreen;

		for (int i = 0; i < resolutions.Count; i++)
		{
			Resolution resolution = resolutions[i];
			if (resolution.width == Settings.settings.width && resolution.height == Settings.settings.height && resolution.refreshRate == Settings.settings.hz)
			{
				resolutionInput.SetValueWithoutNotify(i);
			}
		}
	}

}
