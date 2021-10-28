using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlockSource : MonoBehaviour, IPointerDownHandler
{
	public Image iconImage;
	public Image backgroundImage;
	TimelineEditor editor;
	BlockData template;


	public void InitiateFromBlockData(BlockData template, TimelineEditor editor)
	{
		this.editor = editor;
		this.template = template;
		backgroundImage.color = template.color;
		iconImage.sprite = template.icon;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		//if (!editor.HasSelectedUnit()) return;
		BlockElement blockElement = editor.CreateBlockElement(template);
		blockElement.SetDragging(true);
		editor.DeselectBlock();
	}
}
