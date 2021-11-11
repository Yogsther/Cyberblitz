using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invite
{
	public UserID from;
	public UserID to;
	public Invite() { }
	public Invite(UserID from, UserID to)
	{
		this.from = from;
		this.to = to;
	}

}

public enum InviteStatus
{
	hasInvited,
	hasBeenInvited,
	noInvitations
}


public class Invites
{
	public List<Invite> invites = new List<Invite>();

	public bool CreateInvite(UserID from, UserID to)
	{
		if (!ExistsInvite(from, to))
		{
			invites.Add(new Invite(from, to));
			return true;
		}
		return false;
	}

	public InviteStatus GetInviteStatus(UserID from, UserID to)
	{
		if (ExistsInvite(from, to)) return InviteStatus.hasInvited;
		if (ExistsInvite(to, from)) return InviteStatus.hasBeenInvited;
		return InviteStatus.noInvitations;
	}

	public bool ExistsInvite(UserID from, UserID to)
	{
		foreach (Invite invite in invites) if (invite.from == from && invite.to == to) return true;
		return false;
	}

	public void RemoveAllInvitesRelatingToUser(UserID user)
	{
		foreach (Invite invite in invites.ToArray())
		{
			if (invite.from == user || invite.to == user) invites.Remove(invite);
		}
	}
}
