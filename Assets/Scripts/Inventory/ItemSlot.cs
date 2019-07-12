using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
	public System.Type AllowedItemType;
	public static ItemSlot ItemSlotUnderPointer;
	public static bool ShowDescriptionOnCursorHover = true;

	public Image Icon;
	public ItemSelectionOptionPanel OptionPanel;

	public Item Item;

	public virtual bool SetItem(Item newItem)
	{
		if(newItem == null)
		{
			Item = null;
			Icon.enabled = false;
		}
		else
		{
			if (AllowedItemType != newItem.GetType()) return false;
			Item = newItem;

			Icon.enabled = true;
			Icon.sprite = Item.icon;
		}

		OptionPanel.UpdateUI();
		return true;
	}

	public void OnPointerEnter()
	{
		if(ShowDescriptionOnCursorHover && Item != null)
		{
			Inventory.DescriptionPanel.Show(Item);

			OptionPanel.gameObject.SetActive(true);
		}
		ItemSlotUnderPointer = this;
	}

	public void OnPointerExit()
	{
		Inventory.DescriptionPanel?.Hide();
		OptionPanel.gameObject.SetActive(false);
		ItemSlotUnderPointer = null;
	}

}

