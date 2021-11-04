using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VisualUnit : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
	public UnitID id = "test id";
	public bool friendly = false;

	public Transform mainModel;

	public Animator outlineAnimator;
	public OutlineController outlineController;

	public Animator animator;

	public bool isSelected;
	public bool isSelectable;

	public bool isDead;

	public static Action<UnitID> OnSelected;
	public static Action<UnitID> OnSelectAndDrag;
	public static Action<UnitID> OnDeselected;

	public static Action<UnitID> OnDeath;

	public bool mouseDownOnUnit = false;



	private void Start()
	{
		animator = mainModel.GetComponentInChildren<Animator>();
		SetRagdollEnabled(false);

		outlineAnimator.SetBool("Friendly", friendly);

		TimelineEditor.OnUnitSelected += unit =>
		{
			isSelected = unit.id == id;

			outlineAnimator.SetBool("Selected", isSelected);
		};

		OnDeath += id =>
		{
			if (id == this.id)
			{
				isDead = true;

				SetRagdollEnabled(isDead);

				outlineAnimator.SetBool("Dead", isDead);
			}
		};
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
