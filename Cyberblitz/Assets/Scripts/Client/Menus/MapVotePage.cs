using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapVotePage : MonoBehaviour
{

	public Transform mapParent;
	public MapVoteEntry mapPrefab;
	public MapVotes votes = new MapVotes();
	public MenuSystem menuSystem;
	public List<MapVoteEntry> entries = new List<MapVoteEntry>();

	public Button readyButton;
	public TMP_Text countdown;

	const float VOTE_TIME = 20f;
	float timeLeft = 0f;
	bool ready = false;

	private void Awake()
	{
		MatchManager.OnMapVote += OnVoteUpdate;
		MatchManager.OnMatchStart += match =>
		{
			menuSystem.SetMainMenuVisibility(false);
		};
	}

	void Update()
	{
		if (timeLeft > 0)
		{
			timeLeft -= Time.deltaTime;
			if (timeLeft <= 0)
			{
				timeLeft = 0;
				ReadyUp();
			}

			countdown.text = Mathf.Floor(timeLeft).ToString();
		}
	}

	private void Start()
	{
		readyButton.onClick.AddListener(() =>
		{
			ReadyUp();
		});
	}

	void ReadyUp()
	{
		if (ready) return;
		ready = true;
		Debug.Log("Is ready!");
		MatchManager.SignalReady();
	}

	void OnVoteUpdate(MapVotes votes)
	{
		this.votes = votes;
		Debug.Log("Got map vote, selected screen: " + menuSystem.selectedMenuScreen.name);
		if (menuSystem.selectedMenuScreen.name != "vote")
		{
			StartCoroutine(ShowMapVoteScreen(.5f));
		}
		UpdateMaps();
	}

	public IEnumerator ShowMapVoteScreen(float delay)
	{
		yield return new WaitForSeconds(delay);


		SoundManager.PlaySound("map_selection_start");
		menuSystem.LoadScreen("vote", () =>
		{
			menuSystem.header.SetActive(false);
			menuSystem.gameUI.SetActive(true);
		});

		ready = false;
		timeLeft = VOTE_TIME;

		LoadMaps();
	}

	public void Vote(VoteType type, string map)
	{
		SoundManager.PlaySound("vote_click");
		votes.AddVote(map, ClientLogin.user.id, type);
		ClientConnection.Emit("VOTE", new Vote(map, ClientLogin.user.id, type));
		UpdateMaps();
	}

	public void UpdateMaps()
	{
		foreach (MapVoteEntry entry in entries)
		{
			VoteType userVote = votes.GetUserVote(ClientLogin.user.id, entry.levelData.id);
			int upvotes = votes.GetVotes(entry.levelData.id, VoteType.Upvote);
			int downvotes = votes.GetVotes(entry.levelData.id, VoteType.Downvote);

			entry.UpdateState(userVote, upvotes, downvotes);
		}
	}

	public void LoadMaps()
	{
		ClearMaps();
		foreach (LevelData levelData in LevelManager.levelDataDict.Values)
		{
			MapVoteEntry entry = Instantiate(mapPrefab, mapParent);
			entry.mapVotePage = this;
			entry.Setup(levelData);
			entries.Add(entry);
		}
	}

	void ClearMaps()
	{
		entries.Clear();
		foreach (Transform child in mapParent)
		{
			Destroy(child.gameObject);
		}
	}


}
