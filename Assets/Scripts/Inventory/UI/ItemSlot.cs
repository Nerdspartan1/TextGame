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
	public Image IconEmpty = null;
	public ItemSelectionOptionPanel OptionPanel;

	public Item Item;

	public void SetItem(Item newItem)
	{
		if(newItem == null)
		{
			Item = null;
			if (IconEmpty != null) IconEmpty.enabled = true;

			Icon.enabled = false;
		}
		else
		{
			if (!CanSet(newItem)) throw new System.Exception("Cannot set item : incompatible types");
			Item = newItem;

			if(IconEmpty != null) IconEmpty.enabled = false;
			Icon.enabled = true;
			Icon.sprite = Item.icon;
			Icon.color = Color.white;
		}

		OptionPanel.UpdateUI();
	}

	public bool CanSet(Item item)
	{
		return item.GetType().Equals(AllowedItemType) || item.GetType().IsSubclassOf(AllowedItemType);
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

