using UnityEngine;

public abstract class BlockVisual : MonoBehaviour
{
    //public BlockID blockId;
    public UnitID ownerUnitId;

    public abstract void UpdateVisuals();

}