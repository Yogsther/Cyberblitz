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

	public List<Vote> votes = new List<Vote>();

	public void AddVote(Vote vote)
	{
		AddVote(vote.map, vote.user, vote.type);
	}

	// Only used for the sever
	public void AddVote(string map, UserID user, VoteType type)
	{
		// Remove previous votes
		foreach (Vote vote in votes.ToArray())
			if (vote.user == user)
			{
				if (vote.map == map || (type == VoteType.Downvote && vote.type == VoteType.Downvote))
				{
					// Remove the vote if it's on the same map or if it's a downvote on another map
					votes.Remove(vote);
					// If it's the same vote on the same map
					if (vote.type == type && vote.map == map) return;
				}
			}

		votes.Add(new Vote(map, user, type));
	}

	public VoteType GetUserVote(UserID user, string map)
	{
		foreach (Vote vote in votes)
		{
			if (vote.map == map && user == vote.user) return vote.type;
		}
		return VoteType.Undecided;
	}

	public int GetVotes(string map, VoteType type)
	{
		int mapVotes = 0;
		foreach (Vote vote in votes)
		{
			if (map == vote.map && type == vote.type) mapVotes++;
		}
		return mapVotes;
	}
}
