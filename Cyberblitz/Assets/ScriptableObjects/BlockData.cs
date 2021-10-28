using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BlockType
{
	Move,
	Guard,
	Emote
}


[CreateAssetMenu(fileName = "new_BlockData", menuName = "Block/Data")]
public class BlockData : ScriptableObject
{
	public Sprite icon;

	/// <summary>
	/// Block color in the UI
	/// </summary>
	public Color32 color;
	public new string name;
	public BlockType type;
	public Type blockClass;


	/// <summary>
	/// Can this block be resized in the timeline UI?
	/// </summary>
	public bool resizable = true;

	/// <summary>
	/// The player can move the block position in timeline via the TimelinEditor
	/// </summary>
	public bool moveable = true;

	/// <summary>
	/// Minimum length in seconds
	/// </summary>
	public float minLength = 1;
	/// <summary>
	/// Maximum length in seconds (-1 Infinity)
	/// </summary>
	public float maxLength = -1;

	/// <summary>
	/// If this block is visible in toolbar and can be dragged into the timeline
	/// by the player.
	/// </summary>
	public bool showInToolbar = true;

}
