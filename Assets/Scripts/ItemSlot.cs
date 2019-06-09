using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
	private Image icon;

	public Item item;

	private void Awake()
	{
		icon = transform.GetChild(0).GetComponent<Image>();
	}

	public void SetItem(Item newItem)
	{
		item = newItem;

		icon.enabled = true;
		icon.sprite = item.icon;
		
	}

	public void Clear()
	{
		item = null;

		icon.enabled = false;
	}
}
