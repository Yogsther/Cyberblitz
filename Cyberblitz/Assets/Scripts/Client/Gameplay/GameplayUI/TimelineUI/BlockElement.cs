using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BlockElement : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
	private readonly float START_DRAGGING_PIXEL_THRESHOLD = 15f;
	private readonly float RESIZE_PIXEL_THRESHOLD = 60f;
	private TimelineEditor editor;

	public bool deletable = false;

	public RectTransform rectTransform, resizeHandleRect;

	public Image blockBackgroundImage, iconImage, resizeHandleImage, outline;
	public TMP_Text blockDurationText;

	public Block block;
	[HideInInspector]
	public BlockData template;

	private class DragOperation
	{
		public bool dragging = false;
		/// <summary> If the user has started trying to drag but has not passed the threshold </summary>
		public bool attemptedDrag = false;
		public Vector2 dragOffset = new Vector2(0f, 0f);
		public Vector2 dragOrigin = new Vector2(0f, 0f);
	}

	private readonly DragOperation drag = new DragOperation();

	private class ResizeOperation
	{
		public bool resizing = false;
		public float startX;
		public float startWidth;
		public float maxResizeLength;
	}

	private readonly ResizeOperation resize = new ResizeOperation();

	public void InitiateFromBlockData(BlockData template, TimelineEditor editor)
	{

		this.editor = editor;

		this.template = template;

		blockBackgroundImage.color = template.color;
		/*selectionImage.color = template.color;
*/
		iconImage.sprite = template.icon;

		SetSelected(false);
		SetResizeHandleVisible(false);
		UpdatePhysicalProperties();
	}

	public void SetDragging(bool dragging, bool centeredOffset = true)
	{
		SetSelected(true);
		drag.dragging = dragging;
		drag.dragOffset = new Vector2(rectTransform.rect.width / 2, rectTransform.rect.height / 2);
	}

	public void InitiateFromBlock(Block block, TimelineEditor editor)
	{
		this.block = block;

		InitiateFromBlockData(BlockDataLoader.GetBlockData(block.type), editor);
	}

	public void SetResizeHandleVibility(bool visible)
	{
		if (!template.resizable && visible) return;
		resizeHandleImage.enabled = visible;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (!drag.dragging) editor.SelectBlock(block);
	}

	private bool IsMouseOverResizeArea()
	{
		float relativeMousePositionX = editor.GetMousePositionRelative().x - rectTransform.anchoredPosition.x;
		float resizeHandleStartX = rectTransform.rect.width - (-resizeHandleRect.anchoredPosition.x + resizeHandleRect.rect.width);

		return relativeMousePositionX > resizeHandleStartX;
	}

	private void OnDrag()
	{
		Vector2 localPoint = editor.GetMousePositionRelative();
		rectTransform.SetAsLastSibling();

		localPoint -= drag.dragOffset;
		rectTransform.anchoredPosition = localPoint;


		if (editor.IsMouseIsOverTimeline() && IsPlaceableInTimeline())
		{
			editor.PreviewInsert(editor.GetMousePositionRelative().x);
		} else
		{
			editor.SetMarkerVisibility(false);
		}
	}

	private bool IsPlaceableInTimeline()
	{
		return template.minLength <= editor.freeTimeInTimeline;
	}

	private void OnDragStart()
	{
		if (template.moveable || deletable)
		{
			editor.SelectBlock(block);
			drag.attemptedDrag = false;
			drag.dragging = true;
			drag.dragOffset = editor.GetMousePositionRelative() - rectTransform.anchoredPosition;
			editor.RemoveTimelineBlock(this);
		}
	}

	private void OnDragEnd()
	{
		if (editor.IsMouseIsOverTimeline() && IsPlaceableInTimeline())
		{
			int insertIndex = editor.GetInsertIndex(editor.GetMousePositionRelative().x);

			if (!template.moveable && block != null) insertIndex = block.timelineIndex;

			editor.InsertBlock(this, insertIndex);
			editor.SelectBlock(block);

		} else
		{
			editor.DeselectBlockSafe();
			Destroy(gameObject);
		}
	}

	public void UpdatePhysicalProperties()
	{
		// Set block width from block duration
		float width = editor.TIMELINE_PIXELS_TO_SECONDS;
		if (block != null) width = block.duration * editor.TIMELINE_PIXELS_TO_SECONDS - editor.BLOCK_ELEMENT_PADDING;
		rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);

		// Update block duration-text
		float duration = template.minLength;
		if (block != null) duration = block.duration;
		blockDurationText.text = TimeFormat.TimeToString(duration) + "s";
	}

	public void SetPhysicalPositionInTimeline(float x)
	{
		Vector2 centeredPosition = new Vector2(0f, 0f)
		{
			x = x
		};

		rectTransform.anchoredPosition = centeredPosition;
	}

	public float GetWidth()
	{
		return rectTransform.rect.width;
	}

	private void OnResize()
	{
		if (template.resizable)
		{
			editor.SelectBlock(block);
			float duration = ((editor.GetMousePositionRelative().x - resize.startX) + resize.startWidth) / editor.TIMELINE_PIXELS_TO_SECONDS;
			if (duration > resize.maxResizeLength) duration = resize.maxResizeLength;
			if (duration < template.minLength) duration = template.minLength;
			editor.ResizeBlock(block, duration);
		}

	}



	private void Update()
	{

		if (resize.resizing)
		{
			if (!Mouse.current.leftButton.isPressed)
				resize.resizing = false;
			else OnResize();
		}

		if (drag.attemptedDrag)
		{
			if (!Mouse.current.leftButton.isPressed)
			{
				drag.attemptedDrag = false;
			} else if (Vector2.Distance(drag.dragOrigin, editor.GetMousePositionRelativeCenter()) > START_DRAGGING_PIXEL_THRESHOLD)
			{
				OnDragStart();
			}
		}

		if (drag.dragging)
		{
			if (Mouse.current.leftButton.isPressed)
			{
				OnDrag();
			} else
			{
				drag.dragging = false;
				OnDragEnd();
			}
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (IsMouseOverResizeArea())
		{
			resize.resizing = true;
			resize.startX = editor.GetMousePositionRelative().x;
			resize.startWidth = GetWidth();
			resize.maxResizeLength = editor.freeTimeInTimeline + block.duration;

		} else
		{
			drag.attemptedDrag = true;
			drag.dragOrigin = editor.GetMousePositionRelativeCenter();
		}

	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		SetResizeHandleVibility(true);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		SetResizeHandleVibility(false);
	}

	public float GetStartX()
	{
		return rectTransform.anchoredPosition.x;
	}

	public float GetCenterX()
	{
		return rectTransform.anchoredPosition.x + rectTransform.rect.width / 2;
	}

	public float GetEndX()
	{
		return rectTransform.anchoredPosition.x + rectTransform.rect.width;
	}

	public void SetSelected(bool selected)
	{

		Color32 darkColor = Color.Lerp(template.color, Color.black, .5f);
		blockBackgroundImage.color = selected ? template.color : darkColor;

		/*outline.enabled = selected;*/
		/*outline.color = darkColor;*/
		/*selectionImage.enabled = selected;*/
	}

	private void SetResizeHandleVisible(bool visible)
	{
		resizeHandleImage.enabled = visible;
	}
}
