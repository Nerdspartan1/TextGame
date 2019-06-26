using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Draggable))]
public class DraggableIcon : MonoBehaviour
{
	private ItemSlot itemSlot;
	private Vector2 originalPosition;

	private void Awake()
	{
		itemSlot = GetComponentInParent<ItemSlot>();
		originalPosition = transform.localPosition;
	}

	public void BeginDrag()
	{
		transform.parent = GameManager.Instance.FrontCanvas;
	}

	public void EndDrag()
	{
		if (ItemSlot.ItemSlotUnderPointer)
		{

		}
		transform.parent = itemSlot.transform;
		transform.localPosition = originalPosition;
	}
}
