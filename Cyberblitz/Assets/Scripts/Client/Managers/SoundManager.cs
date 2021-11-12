using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
	public string name;
	public AudioClip clip;
}

public class SoundRequest
{
	public Sound sound;
	public float delay;
	public Vector3 position;
}

public class SoundManager : MonoBehaviour
{

	public float effectVolume;
	static int nextSpeaker = 0;
	static List<AudioSource> speakers = new List<AudioSource>();
	public GameObject speakerPrefab;
	static AudioSource musicPlayer;
	static AudioSource ambiancePlayer;

	public AudioSource musicPlayerReference;
	public AudioSource ambiancePlayerReference;

	public Sound[] sounds;

	public static Sound[] staticSounds;

	static List<SoundRequest> soundQueue = new List<SoundRequest>();

	private void Awake()
	{
		staticSounds = sounds;
		musicPlayer = musicPlayerReference;
		ambiancePlayer = ambiancePlayerReference;

		Settings.OnAudioChanged += () =>
		{
			musicPlayer.volume = Settings.settings.music;
			ambiancePlayer.volume = Settings.settings.ambience;
		};

		for (int i = 0; i < 10; i++)
		{
			GameObject speaker = Instantiate(speakerPrefab, transform);
			speaker.name = "Speaker " + (i + 1);
			AudioSource speakerSource = speaker.GetComponent<AudioSource>();

			speakerSource.volume = effectVolume;

			speakers.Add(speakerSource);
		}
	}

	public static void PlayMusicNonLooping(string name)
	{
		musicPlayer.loop = false;
		musicPlayer.clip = GetSound(name).clip;
		musicPlayer.Play();
	}

	public static void StopMusic()
	{
		musicPlayer.Stop();
	}

	public static void PlayMusic(AudioClip music)
	{
		musicPlayer.clip = music;
		musicPlayer.loop = true;
		musicPlayer.Play();
	}

	public static void PlayAmbience(AudioClip ambience)
	{
		ambiancePlayer.clip = ambience;
		ambiancePlayer.Play();
	}

	public static void PlaySound(string name)
	{
		SoundRequest request = new SoundRequest();
		request.sound = GetSound(name);
		QueueSound(request);
	}

	public static void PlaySound(string name, Vector3 position, float delay = 0f)
	{
		SoundRequest request = new SoundRequest();
		request.sound = GetSound(name);
		request.position = position;
		request.delay = delay;

		QueueSound(request);
	}

	public static void PlaySound(AudioClip clip, Vector3 position, float delay = 0f)
	{
		SoundRequest request = new SoundRequest();
		request.sound = new Sound();
		request.sound.clip = clip;

		request.position = position;
		request.delay = delay;

		QueueSound(request);
	}

	public static void PlaySound(AudioClip clip, float delay = 0f)
	{
		SoundRequest request = new SoundRequest();
		request.sound = new Sound();
		request.sound.clip = clip;

		request.delay = delay;

		QueueSound(request);
	}

	static Sound GetSound(string name)
	{
		foreach (Sound sound in staticSounds)
		{
			if (sound.name == name) return sound;
		}

		Debug.LogWarning("Sound not found: " + name);
		return null;
	}

	static void QueueSound(SoundRequest request)
	{
		soundQueue.Add(request);
	}

	void PlaySound(SoundRequest soundRequest)
	{
		AudioSource speaker = speakers[nextSpeaker];

		if (soundRequest.position != null) speaker.transform.position = soundRequest.position;

		speaker.clip = soundRequest.sound.clip;
		speaker.volume = Settings.settings.effects;

		speaker.Play();

		nextSpeaker++;
		nextSpeaker %= speakers.Count;
	}


	void Update()
	{
		foreach (SoundRequest request in soundQueue.ToArray())
		{
			if (request.delay <= 0)
			{
				PlaySound(request);
				soundQueue.Remove(request);
			} else
			{
				request.delay -= Time.deltaTime * 1000;
			}
		}
	}
}
