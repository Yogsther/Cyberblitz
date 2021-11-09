using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TimelineEditor : InGameEditor
{
	public BlockEditor blockEditor;

	public float TOTAL_TIMELINE_DURATION = 10f;
	public float freeTimeInTimeline;

	Dictionary<BlockType, Type> classFinder = new Dictionary<BlockType, Type> {
			{ BlockType.Move, typeof(MoveBlock) },
			{ BlockType.Guard, typeof(GuardBlock) },
			{ BlockType.Emote, typeof(EmoteBlock) }
	};

	[HideInInspector]
	public float TIMELINE_DURATION = 10f;
	[HideInInspector]
	public float TIMELINE_PIXELS_TO_SECONDS;
	float TIMELINE_WIDTH;

	[HideInInspector]
	public readonly float BLOCK_ELEMENT_PADDING = 5f; // Pixels

	public BlockElement blockElementPrefab;
	List<BlockElement> blockElements = new List<BlockElement>();

	public RectTransform timelineField;
	public GameObject timelineGroup;
	public BlockSources blockSources;

	public Canvas canvas;
	public GameObject markerPrefab;
	RectTransform marker;

	Unit selectedUnit;
	public Block selectedBlock;

	public static Action<Block> OnBlockSelected;
	public static Action<Block> OnBlockUpdate;
	public static Action<UnitID> OnUnitSelected;
	public static Action<UnitID> OnUnitDeselect;
	public static Action OnBlockDeselected;

	private void Start()
	{

		TIMELINE_WIDTH = timelineField.rect.width - BLOCK_ELEMENT_PADDING;
		TIMELINE_PIXELS_TO_SECONDS = TIMELINE_WIDTH / TIMELINE_DURATION;

		VisualUnit.OnSelected += id => SelectUnit(MatchManager.GetUnit(id));
		VisualUnit.OnSelectAndDrag += id => SelectAndDragUnit(MatchManager.GetUnit(id));

		blockSources.CreateSourceButtons(this);

		SetTimelineVisibility(false);
		ArrengeUITimeline();

		MatchManager.OnPlanningEnd += OnPlanningEnd;
	}


	void OnPlanningEnd()
	{
		DeselectUnit();
	}

	Timeline GetSelectedTimeline()
	{
		return selectedUnit.timeline;
	}


	void SetTimelineVisibility(bool visibility)
	{
		timelineGroup.SetActive(visibility);
		blockSources.SetVisibility(visibility);
	}

	public void SelectBlock(Block block)
	{
		DeselectBlock();
		selectedBlock = block;
		GetSelectedBlockElement().SetSelected(true);

		blockEditor.EditBlock(ref block, this);
		blockEditor.OnUpdated += BlockUpdate;
		OnBlockSelected?.Invoke(block);
	}

	public void BlockUpdate()
	{
		ArrengeUITimeline();
		OnBlockUpdate?.Invoke(selectedBlock);
	}

	public void DeselectBlock()
	{
		if (selectedBlock != null)
		{
			GetSelectedBlockElement().SetSelected(false);
			blockEditor.StopEditing();
			DeselectBlockSafe();
		}
		OnBlockDeselected?.Invoke();
	}

	// Safe way to deselect a block if you are not sure if it's inside or outside
	// the timeline.
	// This is used before destroying a block that is being dragged
	public void DeselectBlockSafe()
	{
		selectedBlock = null;
		blockEditor.StopEditing();
		blockEditor.OnUpdated -= BlockUpdate;
	}

	public bool HasSelectedBlock()
	{
		return selectedBlock != null;
	}

	BlockElement GetSelectedBlockElement()
	{
		if (!HasSelectedBlock() || selectedBlock.timelineIndex >= GetSelectedTimeline().GetSize() || selectedBlock.timelineIndex < 0) return null;
		return blockElements[selectedBlock.timelineIndex];
	}

	public bool HasSelectedUnit()
	{
		return selectedUnit != null;
	}

	public void ResizeBlock(Block block, float newDuration)
	{
		block.duration = newDuration;
		BlockUpdate();
	}

	public BlockElement CreateBlockElement(BlockData template)
	{
		BlockElement blockElement = Instantiate(blockElementPrefab, timelineField);
		blockElement.InitiateFromBlockData(template, this);

		return blockElement;
	}



	void ClearTimeline()
	{
		foreach (BlockElement blockElement in blockElements)
		{
			DestroyImmediate(blockElement.gameObject);
		}

		blockElements.Clear();
	}

	private void SelectUnit(Unit unit)
	{
		if (MatchManager.match.state == Match.GameState.Planning)
		{
			DeselectUnit();
			SetTimelineVisibility(true);
			ClearTimeline();
			selectedUnit = unit;
			LoadTimeline();
			OnUnitSelected?.Invoke(GetSelectedUnitID());
		}
	}

	void SelectAndDragUnit(Unit unit)
	{
		if (MatchManager.match.state == Match.GameState.Planning)
		{
			SelectUnit(unit);
			if (GetSelectedTimeline().GetSize() == 0)
				AddNewMoveBlock();
		}

	}


	void LoadTimeline()
	{
		foreach (Block block in GetSelectedTimeline().blocks)
		{
			BlockElement blockElement = CreateBlockElement(block);
			blockElements.Add(blockElement);
		}

		if (GetSelectedTimeline().GetSize() > 0)
			SelectBlock(GetSelectedTimeline().GetLastBlock());

		ArrengeUITimeline();
	}

	void AddNewMoveBlock()
	{
		MoveBlock moveBlock = new MoveBlock(selectedUnit.id, 0);
		BlockElement blockElement = CreateBlockElement(moveBlock);
		InsertBlock(blockElement);
		SelectBlock(moveBlock);
	}


	public BlockElement CreateBlockElement(Block block)
	{
		BlockElement blockElement = Instantiate(blockElementPrefab, timelineField);
		blockElement.InitiateFromBlock(block, this);

		return blockElement;
	}

	/// <summary>
	/// Get the amount of free time left in the selected timeline
	/// </summary>
	/// <returns></returns>
	public float GetFreeTime()
	{
		return -1f;
	}

	/// <summary>
	/// Returns the mouse position relative the the center of the timeline field.
	/// If the mouse is in the center of the timeline field it will return (0,0)
	/// </summary>
	/// <returns></returns>
	public Vector2 GetMousePositionRelativeCenter()
	{
		Vector2 localPoint = InputManager.pointerScreenPosition;

		RectTransformUtility.ScreenPointToLocalPointInRectangle(timelineField, localPoint, null, out localPoint);

		return localPoint;
	}

	/// <summary>
	/// Returns the position of the mouse, relative to the timeline field.
	/// If the mouse is in the bottom left of the timeline the value will be (0,0)
	/// </summary>
	/// <returns></returns>
	public Vector2 GetMousePositionRelative()
	{
		Vector2 relative = GetMousePositionRelativeCenter();

		relative.x = timelineField.rect.width - (timelineField.rect.width / 2 - relative.x);
		relative.y = timelineField.rect.height - (timelineField.rect.height / 2 - relative.y);
		return relative;
	}

	public bool IsMouseIsOverTimeline()
	{
		Vector2 position = GetMousePositionRelative();
		return position.x > 0 && position.x < timelineField.sizeDelta.x &&
			   position.y > 0 && position.y < timelineField.sizeDelta.y;
	}

	public void RemoveTimelineBlock(BlockElement blockElement)
	{
		blockElements.Remove(blockElement);
		GetSelectedTimeline().RemoveBlock(blockElement.block);
		ArrengeUITimeline();
	}


	public bool InsertBlock(BlockElement blockElement, int insertIndex = -1)
	{

		if (blockElement.block == null)
		{
			Type classType = classFinder[blockElement.template.type];
			Block block = (Block)Activator.CreateInstance(classType, selectedUnit.id, GetSelectedTimeline().GetSize());

			blockElement.block = block;
		}

		if (insertIndex == -1 || insertIndex > blockElements.Count) blockElements.Add(blockElement);
		else blockElements.Insert(insertIndex, blockElement);

		GetSelectedTimeline().InsertBlock(blockElement.block, insertIndex);
		ArrengeUITimeline();
		SetMarkerVisibility(false);
		return true;
	}

	public void ArrengeUITimeline()
	{
		float x = 0;
		freeTimeInTimeline = TOTAL_TIMELINE_DURATION;

		BlockElement lastMovementBlock = null;


		for (int i = 0; i < blockElements.Count; i++)
		{
			BlockElement blockElement = blockElements[i];
			blockElement.block.timelineIndex = i;
			blockElement.SetPhysicalPositionInTimeline(x);
			blockElement.UpdatePhysicalProperties();
			if (i != blockElements.Count - 1) x += blockElement.GetWidth() + BLOCK_ELEMENT_PADDING;

			if (blockElement.block.type == BlockType.Move)
			{
				blockElement.deletable = false;
				lastMovementBlock = blockElement;
			}

			freeTimeInTimeline -= blockElement.block.duration;
		}

		// The last movement block can always be deleted but not moved.
		if (lastMovementBlock != null) lastMovementBlock.deletable = true;
	}

	public int GetInsertIndex(float x)
	{
		for (int i = 0; i < blockElements.Count; i++)
		{
			if (x < blockElements[i].GetCenterX()) return i;
			else if (i == blockElements.Count - 1) return i + 1;
		}
		return -1;
	}

	public void PreviewInsert(float x)
	{
		SetMarkerVisibility(true);
		int insertIndex = GetInsertIndex(x);
		if (insertIndex == -1) x = BLOCK_ELEMENT_PADDING / 2;
		else if (insertIndex > blockElements.Count - 1) x = blockElements[blockElements.Count - 1].GetEndX() + BLOCK_ELEMENT_PADDING / 2;
		else x = blockElements[insertIndex].GetStartX() - BLOCK_ELEMENT_PADDING / 2;

		marker.anchoredPosition = new Vector2(x, 0f);
	}

	public void SetMarkerVisibility(bool visible)
	{
		if (marker == null) marker = Instantiate(markerPrefab, timelineField).GetComponent<RectTransform>();
		marker.gameObject.SetActive(visible);
	}


	public UnitID GetSelectedUnitID()
	{
		if (selectedUnit == null) return "";
		return selectedUnit.id;
	}

	public void DeselectUnit()
	{
		OnUnitDeselect?.Invoke(GetSelectedUnitID());
		selectedUnit = null;
		blockEditor.StopEditing();
		DeselectBlockSafe();
		ClearTimeline();
		SetTimelineVisibility(false);
	}
}
