using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitPortrait : MonoBehaviour
{
	public Image crown, roleIcon, unitImage, portraitFrame, deathCross, selectedFrame;
	public Transform healthBlobs;
	public Color32 deadColor;

	public void Setup(UnitType type)
	{
		crown.enabled = type == UnitType.Courier;
		roleIcon.enabled = type != UnitType.Courier;

		UnitData unitData = UnitDataManager.GetUnitDataByType(type);

		roleIcon.sprite = unitData.roleIcon;
		unitImage.sprite = unitData.portrait;

		SetMaxHp(unitData.stats.maxHp);
		SetSelected(false);
		SetAlive();
	}

	public void SetSelected(bool selected)
	{
		selectedFrame.enabled = selected;
	}

	void SetMaxHp(float hp)
	{
		for (int i = 0; i < healthBlobs.childCount; i++)
		{
			Image blob = healthBlobs.GetChild(i).GetComponent<Image>();
			blob.enabled = hp >= i;
		}
	}
	public void SetAlive()
	{
		deathCross.enabled = false;
		crown.color = Color.white;
		roleIcon.color = Color.white;
		unitImage.color = Color.white;
	}

	public void SetDead()
	{
		deathCross.enabled = true;
		crown.color = deadColor;
		roleIcon.color = deadColor;
		unitImage.color = deadColor;
	}

	public void SetHp(float hp)
	{
		for (int i = 0; i < healthBlobs.childCount; i++)
		{
			Image blob = healthBlobs.GetChild(i).GetComponent<Image>();
			blob.color = hp >= i ? Color.white : Color.black;
		}
	}
}
