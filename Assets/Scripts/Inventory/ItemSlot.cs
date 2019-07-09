using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
	public static ItemSlot ItemSlotUnderPointer;

	public Image Icon;
	public ItemSelectionOptionPanel optionPanel;

	public Item Item;

	public void SetItem(Item newItem)
	{
		Item = newItem;
		optionPanel.UpdateUI();

		if(Item != null)
		{
			Icon.enabled = true;
			Icon.sprite = Item.icon;
		}
		else
		{
			Icon.enabled = false;
		}

	}

	public void OnPointerEnter()
	{
		if(Item != null)
		{
			Inventory.DescriptionPanel.Show(Item);

			optionPanel.gameObject.SetActive(true);
		}
		ItemSlotUnderPointer = this;
	}

	public void OnPointerExit()
	{
		Inventory.DescriptionPanel?.Hide();
		optionPanel.gameObject.SetActive(false);
		ItemSlotUnderPointer = null;
	}

}
