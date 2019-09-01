using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MerchantPanel : MonoBehaviour
{
	public Merchant Merchant;

	[Header("Prefabs")]
	public ItemSlot ItemSlotPrefab;

	[Header("References")]
	public Transform ItemsPanel;
	public Text MerchantName;

	public void Start() //DEBUG
	{
		UpdateUI();
	}

	public void SetMerchant(Merchant merchant)
	{
		Merchant = merchant;
		UpdateUI();
	}

	public void UpdateUI()
	{
		MerchantName.text = Merchant.Name;
		//create slots
		foreach(Transform child in ItemsPanel)
		{
			Destroy(child.gameObject);
		}
		foreach (var item in Merchant.Items)
		{
			var itemSlot = Instantiate(ItemSlotPrefab, ItemsPanel).GetComponent<ItemSlot>();
			itemSlot.AllowedItemType = typeof(Item);
			itemSlot.SetItem(item);
		}
	}
}
