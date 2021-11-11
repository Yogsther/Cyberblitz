using UnityEngine;

public abstract class VisualBlock : MonoBehaviour
{
    public Block block;

    public bool selected;

    protected Animator animator;

    public bool isSet = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("Selected", selected);

        BlockEditor.OnBlockSet += id =>
        {
            if (id == block.id)
            {
                isSet = true;
            }
        };
    }

    public virtual void SetBlock(Block block)
    {
        this.block = block;
    }

    public virtual void SetSelected(bool status)
    {
        selected = status;
        animator.SetBool("Selected", selected);
    }

    public abstract void UpdateVisuals();

}