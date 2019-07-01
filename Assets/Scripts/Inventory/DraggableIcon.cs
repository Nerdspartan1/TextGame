using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Draggable),typeof(Image))]
public class DraggableIcon : MonoBehaviour
{
	private ItemSlot itemSlot;
	private Image image;
	private Vector2 originalPosition;

	public void Awake()
	{
		itemSlot = GetComponentInParent<ItemSlot>();
		image = GetComponent<Image>();
		originalPosition = transform.localPosition;
	}

	public void BeginDrag()
	{
		transform.SetParent(GameManager.Instance.FrontCanvas);
		image.raycastTarget = false;
	}

	public void EndDrag()
	{
		if (ItemSlot.ItemSlotUnderPointer)
			itemSlot.Swap(ItemSlot.ItemSlotUnderPointer);

		transform.SetParent(itemSlot.transform);
		transform.localPosition = originalPosition;
		image.raycastTarget = true;
	}
}
