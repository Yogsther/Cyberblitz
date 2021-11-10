using UnityEngine;

public abstract class VisualBlock : MonoBehaviour
{
    public Block block;

    public bool selected;

    protected Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("Selected", selected);
    }

    public virtual void SetSelected(bool status)
    {
        selected = status;
        animator.SetBool("Selected", selected);
    }

    public abstract void UpdateVisuals();

}