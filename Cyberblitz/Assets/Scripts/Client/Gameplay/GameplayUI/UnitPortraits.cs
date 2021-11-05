using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPortraits : MonoBehaviour
{
	public UnitPortrait[] portraits;

	void Start()
	{
		MatchManager.OnMatchStart += OnMatchStart;
		MatchManager.OnMatchUpdate += OnMatchUpdate;
	}

	void OnMatchStart(Match match)
	{
		Unit[] units = match.GetAllUnits(match.GetLocalTeam());
		for (int i = 0; i < units.Length; i++)
		{
			portraits[i].Setup(units[i].type);
		}
	}

	void OnMatchUpdate(Match match)
	{
		Unit[] units = match.GetAllUnits(match.GetLocalTeam());
		for (int i = 0; i < units.Length; i++)
		{
			portraits[i].SetHp(units[i].hp);
			if (units[i].IsDead()) portraits[i].SetDead();
		}

	}



	void Update()
	{

	}
}
