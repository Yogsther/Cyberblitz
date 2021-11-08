using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VoteType
{
	Undecided,
	Upvote,
	Downvote
}

public class Vote
{
	public VoteType type;
	public string map;
	public UserID user;

	public Vote() { }
	public Vote(string map, UserID user, VoteType type)
	{
		this.map = map;
		this.user = user;
		this.type = type;
	}
}

public class MapVotes
{
	public int votingUsers = 2;
	public List<Vote> votes = new List<Vote>();

	/*public MapVotes() { }*/

	// Only used for the sever
	public void AddVote(string map, UserID user, VoteType type)
	{
		// Remove previous votes
		foreach (Vote vote in votes.ToArray())
			if (vote.map == map) votes.Remove(vote);

		votes.Add(new Vote(map, user, type));
	}

	public VoteType GetUserVote(string map)
	{
		foreach (Vote vote in votes)
		{
			if (vote.map == map) return vote.type;
		}
		return VoteType.Undecided;
	}

	public int GetVotes(string map, VoteType type)
	{
		int mapVotes = 0;
		foreach (Vote vote in votes)
		{
			if (vote.type == type) mapVotes++;
		}
		return mapVotes;
	}
}
