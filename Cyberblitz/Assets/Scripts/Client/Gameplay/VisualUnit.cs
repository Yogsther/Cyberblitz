using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.VFX;

public class VisualUnit : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public UnitID id = "test id";
    public UnitType type => MatchManager.GetUnit(id).type;
    public UnitData unitData => UnitDataManager.GetUnitDataByType(type);

    public bool friendly = false;

    public Transform modelTransform;
    public GameObject model;

    public float rotationOffset;
    public float aimRotationOffset = -44.2f;

    public Quaternion modelRotation => Quaternion.AngleAxis(rotationOffset, Vector3.down);
    public Quaternion modelAimRotation => Quaternion.AngleAxis(aimRotationOffset, Vector3.down);

    public bool lockTargetForward = false;

    public Animator outlineAnimator;
    public OutlineController outlineController;

    public Animator animator;

    public VisualEffect shootVFX;

    public SpriteRenderer overheadIconRenderer;

    public bool isSelected;
    public bool isSelectable;

    public bool isDead;

    public static Action<UnitID> OnSelected;
    public static Action<UnitID> OnSelectAndDrag;
    public static Action<UnitID> OnDeselected;

    public static Action<UnitID, bool> OnShoot;

    public static Action<UnitID> OnDeath;

    public bool mouseDownOnUnit = false;

    private Vector3 targetForward;

    public Rigidbody[] rigidBodies;

    private readonly float smoothRotationVelocity;

    public bool isAiming;
    private readonly VisualUnit targetVisualUnit;

    private void Start()
    {
        animator = modelTransform.GetComponentInChildren<Animator>();
        shootVFX = modelTransform.GetComponentInChildren<VisualEffect>();
        SetRagdollEnabled(false);

        outlineAnimator.SetBool("Friendly", friendly);

        TimelineEditor.OnUnitSelected += id =>
        {
            isSelected = id == this.id;
            // TODO NULL CHECK BECAUSE BUG IF YOU PLAY MORE THAN ONE GAME
            if (outlineAnimator != null)
            {
                outlineAnimator.SetBool("Selected", isSelected);
            }
        };

        TimelineEditor.OnUnitDeselect += id =>
        {
            bool isDeselectedUnit = id == this.id;

            if (isDeselectedUnit)
            {
                isSelected = false;
                if (outlineAnimator != null)
                {
                    outlineAnimator.SetBool("Selected", isSelected);
                }
            }
        };

        OnShoot += (id, hit) =>
        {
            if (id == this.id)
            {
                if (hit)
                {
                    SoundManager.PlaySound(unitData.GetRandomFireSound(), modelTransform.position);
                }
                else
                {
                    SoundManager.PlaySound("missed_shot", modelTransform.position);
                }

                SoundManager.PlaySound("shell_drop", modelTransform.position, 500f);

                animator.SetTrigger("FireTrigger");

                shootVFX.SendEvent("Fire");
            }
        };

        OnDeath += id =>
        {

            if (id == this.id)
            {
                isDead = true;
                isSelectable = false;
                SetRagdollEnabled(isDead);

                outlineAnimator.SetBool("Dead", isDead);
                SoundManager.PlaySound("unit_killed", modelTransform.position);
            }
        };
    }



    private void Update()
    {
        model.transform.localRotation = Quaternion.RotateTowards(model.transform.localRotation, isAiming ? modelAimRotation : modelRotation, 180f * Time.deltaTime);
        float angleDifference = Vector3.Angle(modelTransform.forward, targetForward);
        modelTransform.forward = Vector3.RotateTowards(modelTransform.forward, targetForward, (1f + (angleDifference * .1f)) * Time.deltaTime, 1f);

    }

    public void SetVisable(bool visable)
    {

    }

    public void SetTargetForward(Vector3 newTargetForward)
    {
        targetForward = newTargetForward.normalized;
    }

    public void SetRagdollEnabled(bool enabled)
    {

        if (animator != null)
        {
            animator.enabled = !enabled;
        }

        foreach (Rigidbody rb in rigidBodies)
        {
            if (rb != null)
            {
                rb.isKinematic = !enabled;
            }
        }
    }

    public void SetSelected(bool selected)
    {
        if (isSelectable && selected)
        {
            //outlineController.color = selectedColor;
            OnSelected?.Invoke(id);
        }
        else if (isSelected)
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
            if (isSelectable)
            {
                OnSelectAndDrag?.Invoke(id);
            }

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
