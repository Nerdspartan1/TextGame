using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
	[Header("Prefabs")]
	public GameObject DescriptionPanelPrefab;
	public GameObject ItemSlotInstance;

	[Header("Properties")]
	Inventory inventory;
	static public DescriptionPanel DescriptionPanel;

	ItemSlot[] slots;

	public void Awake()
	{
		DescriptionPanel = Instantiate(DescriptionPanelPrefab, GameManager.Instance.Canvas).GetComponent<DescriptionPanel>();
		DescriptionPanel.Hide();
	}

	public void Start()
	{
		inventory = Inventory.Instance;
		inventory.onItemChanged += UpdateUI;

		//create (size-1) slots, because the 1st one already exists
		for(int i = 1; i < inventory.Size; i++)
			Instantiate(ItemSlotInstance, transform);

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
