using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Waypoint : MonoBehaviour, IPointerDownHandler
{
	public int index = -1;
	public Block block;
	public Canvas canvas;

	public Color32 selectedColor, waypointColor;
	public Image image;


	public void OnPointerDown(PointerEventData eventData)
	{
		Debug.Log("Registered click");
		GameManager.instance.TimelineEditor.SelectBlock(block);
	}

	public void SetSelected(bool selected)
	{
		image.color = selected ? selectedColor : waypointColor;
	}

	public void SetCamera(Camera camera)
	{
		canvas.worldCamera = camera;
	}

}
