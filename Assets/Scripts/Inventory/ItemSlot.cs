using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{

	public Image icon;
	public ItemSelectionOptionPanel optionPanel;

	public Item Item;

	public void SetItem(Item newItem)
	{
		Item = newItem;
		optionPanel.Item = Item;
		optionPanel.UpdateUI();

		icon.enabled = true;
		icon.sprite = Item.icon;
		
	}

	public void Clear()
	{
		Item = null;
		optionPanel.Item = null;
		optionPanel.UpdateUI();

		icon.enabled = false;
	}

	public void OnPointerEnter()
	{
		if(Item != null)
		{
			InventoryUI.DescriptionPanel.Show(Item);

			optionPanel.gameObject.SetActive(true);
		}
	}

	public void OnPointerExit()
	{
		InventoryUI.DescriptionPanel?.Hide();
		optionPanel.gameObject.SetActive(false);
	}

}
