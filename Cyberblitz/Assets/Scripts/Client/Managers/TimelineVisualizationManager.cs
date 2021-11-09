using System.Collections.Generic;
using UnityEngine;

public class TimelineVisualizationManager : MonoBehaviour
{

    public LineRenderer moveBlockLine, tempCone, lockedConePrefab;
    public VisualMoveBlock visualMoveBlockPrefab;
    public VisualGuardBlock visualGuardBlockPrefab;
    public Waypoint waypointPrefab;
    public Camera camera;
    public Transform visualBlocksParent, waypointParent, lockedConesParent;
    private Unit selectedUnit;
    private Block selectedBlock;
    private readonly List<VisualBlock> visualBlocks = new List<VisualBlock>();
    private readonly List<Waypoint> waypoints = new List<Waypoint>();

    private void Awake()
    {
        TimelineEditor.OnBlockUpdate += OnBlockUpdated;
        TimelineEditor.OnUnitSelected += OnUnitSelected;
        TimelineEditor.OnBlockSelected += OnBlockSelected;
        TimelineEditor.OnBlockDeselected += OnBlockDeselected;
        MatchManager.OnMatchUpdate += OnMatchUpdate;
    }

    private void OnBlockDeselected()
    {
        selectedBlock = null;
        OnBlockUpdated();
    }

    private void OnMatchUpdate(Match match)
    {
        if (match.state != Match.GameState.Planning)
        {
            selectedBlock = null;
            ClearVisualElements();
        }
    }

    private void ClearVisualElements()
    {
        tempCone.positionCount = 0;
        //moveBlockLine.positionCount = 0;
        ClearTransform(visualBlocksParent);
        ClearTransform(waypointParent);
        ClearTransform(lockedConesParent);

        visualBlocks.Clear();
        waypoints.Clear();
    }

    private void ClearTransform(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    private void LoadMoveBlock(MoveBlock block)
    {
        VisualMoveBlock visualMoveBlock = Instantiate(visualMoveBlockPrefab, visualBlocksParent);
        visualMoveBlock.block = block;

        visualMoveBlock.Init();

        visualBlocks.Add(visualMoveBlock);
    }

    private void LoadGuardBlock(GuardBlock block)
    {
        VisualGuardBlock visualGuardBlock = Instantiate(visualGuardBlockPrefab, visualBlocksParent);
        visualGuardBlock.block = block;
        visualBlocks.Add(visualGuardBlock);
    }

    private void OnUnitSelected(UnitID id)
    {
        selectedUnit = MatchManager.GetUnit(id);
        OnBlockUpdated();
    }

    private void OnBlockSelected(Block block)
    {
        selectedBlock = block;
        OnBlockUpdated(block);

        foreach (VisualBlock visualBlock in visualBlocks)
        {
            bool isSelectedBlock = visualBlock.block == selectedBlock;

            visualBlock.SetSelected(isSelectedBlock);
        }
    }

    private void OnBlockUpdated(Block editedBlock = null)
    {

        RefreshVisualBlocks();

        foreach (VisualBlock visualBlock in GetUnitVisualBlocks(selectedUnit))
        {
            visualBlock.UpdateVisuals();
        }
    }

    private void RefreshVisualBlocks()
    {

        int team = MatchManager.match.GetLocalTeam();
        Unit[] units = MatchManager.match.GetAllUnits(team);



        foreach (Unit unit in units)
        {
            foreach (Block block in unit.timeline.blocks)
            {
                if (!TryGetVisualBlock(block.id, out VisualBlock existingVisualBlock))
                {
                    if (block is MoveBlock)
                    {
                        LoadMoveBlock(block as MoveBlock);
                    }

                    if (block is GuardBlock)
                    {
                        LoadGuardBlock(block as GuardBlock);
                    }
                }
            }
        }
    }

    private bool TryGetVisualBlock(BlockID id, out VisualBlock visualBlockResult)
    {
        visualBlockResult = null;
        foreach (VisualBlock visualBlock in visualBlocks)
        {
            if (visualBlock.block.id == id)
            {
                visualBlockResult = visualBlock;
                break;
            }
        }


        return visualBlockResult != null;
    }

    private List<VisualBlock> GetUnitVisualBlocks(Unit unit)
    {
        List<VisualBlock> unitVisualBlocks = new List<VisualBlock>();

        foreach (VisualBlock visualBlock in visualBlocks)
        {
            if (visualBlock.block.ownerId == unit.id)
            {
                unitVisualBlocks.Add(visualBlock);
            }
        }

        return unitVisualBlocks;
    }
}
