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

	static int nextSpeaker = 0;
	static List<AudioSource> speakers = new List<AudioSource>();
	public GameObject speakerPrefab;
	static AudioSource musicPlayer;

	public Sound[] sounds;
	public Sound[] music;
	public static Sound[] staticSounds;
	public static Sound[] staticMusic;
	static List<SoundRequest> soundQueue = new List<SoundRequest>();

	private void Awake()
	{
		staticSounds = sounds;
		staticMusic = music;
		musicPlayer = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
		for (int i = 0; i < 10; i++)
		{
			GameObject speaker = Instantiate(speakerPrefab, transform);
			speaker.name = "Speaker " + (i + 1);
			speakers.Add(speaker.GetComponent<AudioSource>());
		}
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

		Debug.Log("Played sound " + soundRequest.sound.name);

		if (soundRequest.position != null) speaker.transform.position = soundRequest.position;
		else
		{
			Debug.Log("Playing 2D sound");
			// CHANGE SOUND TYPE 
		}
		speaker.clip = soundRequest.sound.clip;
		speaker.Play();

		nextSpeaker++;
		nextSpeaker %= speakers.Count;
	}



	public static void PlaySound(string name, Vector3 position)
	{

	}

	void Start()
	{

	}

	void Update()
	{
		foreach (SoundRequest request in soundQueue)
		{
			if (request.delay <= 0)
			{
				PlaySound(request);
			} else
			{
				request.delay -= Time.deltaTime;
			}
		}
	}
}
