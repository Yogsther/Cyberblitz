using System.Collections.Generic;
using UnityEngine;

public class TimelineVisualizationManager : MonoBehaviour
{

    public LineRenderer moveBlockLine, tempCone, lockedConePrefab;
    public VisualMoveBlock visualMoveBlockPrefab;
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

    private void CreateNewLockedCone(Vector2[] points)
    {
        LineRenderer line = Instantiate(lockedConePrefab, lockedConesParent);
        DrawCone(points, line);
    }

    private void DrawCone(Vector2[] points, LineRenderer line)
    {
        float height = .2f;
        int coneResolution = 15;

        line.positionCount = points.Length + 1; // Add one for the closing point at the end
        line.positionCount += coneResolution;

        // Draw the first two positions, from unit position to the outer left point of the cone
        for (int i = 0; i < 2; i++)
        {
            line.SetPosition(i, points[i].ToFlatVector3(height));
        }

        Vector2 startDiration = points[1] - points[0];
        Vector2 endDirection = points[2] - points[0];
        float startAngle = Mathf.Atan2(startDiration.y, startDiration.x);
        float endAngle = Mathf.Atan2(endDirection.y, endDirection.x);

        float coneDistance = Vector2.Distance(points[0], points[1]);

        // Angle step is the resolution of the curved part of the cone
        float angleStep = (endAngle - startAngle) / coneResolution;

        for (int i = 0; i < coneResolution; i++)
        {
            float angle = Mathf.LerpAngle(startAngle * Mathf.Rad2Deg, endAngle * Mathf.Rad2Deg, i / (float)coneResolution) * Mathf.Deg2Rad;
            float x = (Mathf.Cos(angle) * coneDistance) + points[0].x;
            float y = (Mathf.Sin(angle) * coneDistance) + points[0].y;
            line.SetPosition(i + 2, new Vector2(x, y).ToFlatVector3(height));
        }

        line.SetPosition(line.positionCount - 2, points[2].ToFlatVector3(height));
        line.SetPosition(line.positionCount - 1, points[0].ToFlatVector3(height));
    }

    private void LoadMoveBlock(MoveBlock block)
    {
        VisualMoveBlock visualMoveBlock = Instantiate(visualMoveBlockPrefab, visualBlocksParent);
        visualMoveBlock.block = block;

        visualMoveBlock.Init();

        visualBlocks.Add(visualMoveBlock);
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
                        GuardBlock guardBlock = (GuardBlock)block;
                        if (guardBlock.aimCone != null && guardBlock.aimCone.isSet)
                        {
                            Vector2[] points = guardBlock.aimCone.GetConePoints(guardBlock.aimCone.direction);
                            CreateNewLockedCone(points);
                        }
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

    private void Update()
    {
        if (selectedBlock != null)
        {
            if (selectedBlock.type == BlockType.Guard)
            {
                GuardBlock guardBlock = (GuardBlock)selectedBlock;
                float inputDirection = 0;
                if (!InputManager.isOnGui && InputManager.TryGetPointerHitLayer(LayerMask.GetMask("Ground"), out RaycastHit groundHit))
                {

                    Vector2 mouseHitPoint = groundHit.point.FlatVector3ToVector2();

                    Vector2 toMouse = mouseHitPoint - guardBlock.aimCone.origin.point;

                    inputDirection = (Mathf.Atan2(toMouse.y, toMouse.x) * Mathf.Rad2Deg) - 90f;

                    Vector2[] points = guardBlock.aimCone.GetConePoints(inputDirection);
                    DrawCone(points, tempCone);
                }
                else
                {
                    tempCone.positionCount = 0;
                }
            }

        }

    }
}
