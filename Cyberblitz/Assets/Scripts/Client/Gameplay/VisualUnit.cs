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

	public OutlineController outlineController;

	[ColorUsage(true, true)] public Color friendlyColor;
	[ColorUsage(true, true)] public Color enemyColor;
	[ColorUsage(true, true)] public Color selectedColor;
	[ColorUsage(true, true)] public Color hoverColor;

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
		SetRagdollEnabled(false);

		outlineController.color = friendlyColor;

		OnSelected += (id) => outlineController.color = id == this.id ? selectedColor : friendlyColor;
	}

	public void SetVisable(bool visable)
	{

	}

	public void SetRagdollEnabled(bool enabled)
	{
		animator.enabled = !enabled;

		foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
		{
			rb.isKinematic = !enabled;
		}
	}

	public void SetSelected(bool selected)
	{
		if (isSelectable && selected)
		{
			//outlineController.color = selectedColor;
			OnSelected?.Invoke(id);
		} else if (isSelected)
		{
			//outlineController.color = friendlyColor;
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
			if (isSelectable) OnSelectAndDrag?.Invoke(id);
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
