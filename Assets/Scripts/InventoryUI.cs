using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
	Inventory inventory;

	public GameObject ItemSlotPrefab;

	ItemSlot[] slots;

	public void Start()
	{
		inventory = Inventory.Instance;
		inventory.onItemChanged += UpdateUI;

		//create the slots
		foreach (Transform child in transform)
			Destroy(child.gameObject);
		for(int i = 0; i < inventory.Size; i++)
			Instantiate(ItemSlotPrefab, transform);

		slots = GetComponentsInChildren<ItemSlot>();

		UpdateUI();
	}

	public void UpdateUI()
	{
		int itemCount = inventory.items.Count;
		for(int i = 0; i < slots.Length; i++)
		{
			if(i < itemCount)
			{
				slots[i].SetItem(inventory.items[i]);
			}
			else
			{
				slots[i].Clear();
			}
		}
	}
}
