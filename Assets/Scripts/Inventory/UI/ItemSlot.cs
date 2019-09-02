using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class ItemSlot : MonoBehaviour
{
	public System.Type AllowedItemType;
	public static ItemSlot ItemSlotUnderPointer;
	public static bool ShowDescriptionOnCursorHover = true;

	public Image Icon;
	public Image IconEmpty = null;
	public ItemSelectionOptionPanel OptionPanel;
	public bool CanBeSwapped = true;

	public Item Item;

	public UnityEvent OnItemChanged;

	public void SetItem(Item newItem)
	{
		bool itemChanged = (newItem != Item);
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

		if(itemChanged) OnItemChanged.Invoke();

		OptionPanel.UpdateUI();
	}

	public bool CanSet(Item item)
	{
		return item.GetType().Equals(AllowedItemType) || item.GetType().IsSubclassOf(AllowedItemType);
	}

	public void OpenOptions()
	{
		if(ShowDescriptionOnCursorHover && Item != null)
		{
			Inventory.DescriptionPanel.Show(Item);

			OptionPanel.gameObject.SetActive(true);
			OptionPanel.UpdateUI();
		}
		ItemSlotUnderPointer = this;
	}

	public void CloseOptions()
	{
		Inventory.DescriptionPanel?.Hide();
		OptionPanel.gameObject.SetActive(false);
		ItemSlotUnderPointer = null;
	}

}

