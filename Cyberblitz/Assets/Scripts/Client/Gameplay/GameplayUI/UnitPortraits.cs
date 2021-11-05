using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPortraits : MonoBehaviour
{
	public UnitPortrait[] portraits;


	void Awake()
	{
		MatchManager.OnMatchStart += OnMatchStart;
		MatchManager.OnMatchUpdate += OnMatchUpdate;
		TimelineEditor.OnUnitSelected += OnUnitSelected;
		TimelineEditor.OnUnitDeselect += OnUnitDeselected;
		CinematicCamera.OnActionCameraIn += OnActionCameraIn;
		CinematicCamera.OnActionCameraOut += OnActionCameraOut;
	}

	void OnActionCameraIn(UnitID id)
	{
		SetPortraitHighlighted(id, true);
	}

	void OnActionCameraOut(UnitID id)
	{
		SetPortraitHighlighted(id, false);
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

	void SetPortraitHighlighted(UnitID id, bool highlighted)
	{
		Match match = MatchManager.match;
		Unit[] units = match.GetAllUnits(match.GetLocalTeam());
		for (int i = 0; i < units.Length; i++)
		{
			if (units[i].id == id) portraits[i].SetHighlighted(highlighted);
		}
	}

	void OnMatchStart(Match match)
	{
		Debug.Log("ON MATCH STARTED, SETTING UP PORTRAITS!");
		Unit[] units = match.GetAllUnits(match.GetLocalTeam());
		Debug.Log("GOT UNITS: " + units.Length);
		for (int i = 0; i < units.Length; i++)
		{
			Debug.Log("SET UP PORTRAIT FOR UNIT");
			portraits[i].Setup(units[i].type, units[i].id);
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
