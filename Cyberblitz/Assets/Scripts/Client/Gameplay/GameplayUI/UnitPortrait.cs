using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitPortrait : MonoBehaviour
{
	public Button selectButton;
	public Image crown, roleIcon, unitImage, portraitFrame, deathCross, selectedFrame, highlightedFrame;
	public Transform healthBlobs;
	public Color32 deadColor;
	public UnitID unitID = new UnitID("NOT_ASSIGNED");

	void Start()
	{
		selectButton.onClick.AddListener(() =>
		{
			Debug.Log("Selecting unit " + unitID);
			VisualUnitManager.GetVisualUnitById(unitID).SetSelected(true);
		});
	}

	public void Setup(UnitType type, UnitID unitID)
	{
		Debug.Log("Setting up Unit: " + type.ToString());
		this.unitID = unitID;
		crown.enabled = type == UnitType.Courier;
		roleIcon.enabled = type != UnitType.Courier;

		UnitData unitData = UnitDataManager.GetUnitDataByType(type);

		roleIcon.sprite = unitData.roleIcon;
		unitImage.sprite = unitData.portrait;
		SetMaxHp(unitData.stats.maxHp);
		SetSelected(false);
		SetHighlighted(false);
		SetAlive();
	}

	public void SetSelected(bool selected)
	{
		selectedFrame.enabled = selected;
	}

	public void SetHighlighted(bool selected)
	{
		highlightedFrame.enabled = selected;
	}

	void SetMaxHp(float hp)
	{
		for (int i = 0; i < healthBlobs.childCount; i++)
		{
			Image blob = healthBlobs.GetChild(i).GetComponent<Image>();
			blob.enabled = hp > i;
		}
	}
	public void SetAlive()
	{
		Debug.Log("Set portrait to ALIVE");
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
			blob.color = hp > i ? Color.white : Color.black;
		}
	}
}
