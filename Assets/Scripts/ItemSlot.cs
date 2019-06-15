using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
	private Image icon;

	public Item Item;

	private void Awake()
	{
		icon = transform.GetChild(0).GetComponent<Image>();
	}

	public void SetItem(Item newItem)
	{
		Item = newItem;

		icon.enabled = true;
		icon.sprite = Item.icon;
		
	}

	public void Clear()
	{
		Item = null;

		icon.enabled = false;
	}

	public void ShowDescription()
	{
		if(Item != null)
		{
			InventoryUI.DescriptionPanel.Show(Item);
		}
	}

	public void HideDescription()
	{
		InventoryUI.DescriptionPanel.Hide();
	}
}
