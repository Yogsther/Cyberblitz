using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BlockSource : MonoBehaviour, IPointerDownHandler
{
	public TMP_Text hotkeyNumberText;
	public Image iconImage;
	public Image backgroundImage;
	TimelineEditor editor;
	BlockData template;

	int hotkey;

	Dictionary<int, Key> keys = new Dictionary<int, Key> {
		{ 1, Key.Digit1 },
		{ 2, Key.Digit2 },
		{ 3, Key.Digit3 }
		 };


	public void InitiateFromBlockData(BlockData template, TimelineEditor editor, int hotkey)
	{
		this.hotkey = hotkey;
		this.editor = editor;
		this.template = template;
		backgroundImage.color = template.color;
		iconImage.sprite = template.icon;
		hotkeyNumberText.text = hotkey.ToString();
	}

	private void Update()
	{
		if (Keyboard.current[keys[hotkey]].wasPressedThisFrame)
		{
			BlockElement blockElement = CreateBlockElement();
			editor.InsertBlock(blockElement);
			editor.SelectBlock(blockElement.block);
		}
	}

	BlockElement CreateBlockElement()
	{
		return editor.CreateBlockElement(template);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		//if (!editor.HasSelectedUnit()) return;
		BlockElement blockElement = CreateBlockElement();
		blockElement.SetDragging(true);
		editor.DeselectBlock();
	}
}
