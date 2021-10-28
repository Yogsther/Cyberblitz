using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VisualUnit : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
	public UnitID id = "test id";
	public Transform mainModel;
	public Transform ghostModel;

	public Animator animator;

	public bool isSelected;
	public bool isSelectable;

	public static Action<UnitID> OnSelected;
	public static Action<UnitID> OnSelectAndDrag;
	public static Action<UnitID> OnDeselected;

	public bool mouseDownOnUnit = false;



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

	public void OnPointerExit(PointerEventData eventData)
	{
		if (mouseDownOnUnit)
		{
			OnSelectAndDrag?.Invoke(id);
			mouseDownOnUnit = false;
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		mouseDownOnUnit = true;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		mouseDownOnUnit = false;
	}
}
