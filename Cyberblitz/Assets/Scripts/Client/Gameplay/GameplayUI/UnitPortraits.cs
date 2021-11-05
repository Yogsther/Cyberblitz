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
		TimelineEditor.OnUnitSelected += OnUnitSelected;
		TimelineEditor.OnUnitDeselect += OnUnitDeselected;
	}

	void OnUnitSelected(UnitID id)
	{
		SetPortraitSelected(id, true);
	}

	void OnUnitDeselected(UnitID id)
	{
		SetPortraitSelected(id, false);
	}

	void SetPortraitSelected(UnitID id, bool selected)
	{
		Match match = MatchManager.match;
		Unit[] units = match.GetAllUnits(match.GetLocalTeam());
		for (int i = 0; i < units.Length; i++)
		{
			if (units[i].id == id) portraits[i].SetSelected(selected);
		}
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
