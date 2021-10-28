using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VisualUnit : MonoBehaviour, IPointerClickHandler
{
	public UnitID id = "test id";
	public Transform mainModel;
	public Transform ghostModel;

	public Animator animator;

	public bool isSelected;
	public bool isSelectable;

	public static Action<UnitID> OnSelected;
	public static Action<UnitID> OnDeselected;

	private void Start()
	{
		animator = mainModel.GetComponentInChildren<Animator>();
		ghostModel.gameObject.SetActive(false);
	}

	public void SetVisable(bool visable)
	{

	}

	public void SetSelected(bool selected)
	{
		if (isSelectable && selected)
		{
			OnSelected?.Invoke(id);
		} else if (isSelected)
		{
			OnDeselected?.Invoke(id);
		}

	}

	public void OnPointerClick(PointerEventData eventData)
	{
		SetSelected(true);
	}
}
