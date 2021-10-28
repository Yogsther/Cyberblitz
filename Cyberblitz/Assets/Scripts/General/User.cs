using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User
{
	public UserID id = UserID.New;
	public string username;
	public int xp, level;
	public bool bot;
	public bool playable; // If the user can be played (only used during alpha, will be deleted.)
}
