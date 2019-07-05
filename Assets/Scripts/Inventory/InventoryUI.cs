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
	public static DescriptionPanel DescriptionPanel;

	private ItemSlot[] slots;

	public void Awake()
	{
		DescriptionPanel = Instantiate(DescriptionPanelPrefab, GameManager.Instance.FrontCanvas).GetComponentInChildren<DescriptionPanel>();
		DescriptionPanel.Hide();
	}

	public void Start()
	{
		inventory = Inventory.Instance;

		//create (size-1) slots, because the 1st one already exists
		for(int i = 1; i < inventory.Size; i++)
			Instantiate(ItemSlotInstance, transform);

		slots = GetComponentsInChildren<ItemSlot>();

		for (int i = 0; i < inventory.Size; i++)
		{
			slots[i].SetItem(inventory[i]);
		}

		inventory.onItemChanged += delegate (int i) { slots[i].SetItem(inventory[i]); };

	}

}
