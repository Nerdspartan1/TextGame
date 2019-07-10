using System.Collections.Generic;
using UnityEngine;

public enum Hand
{
	Right = 0,
	Left = 1
}

public class Inventory : MonoBehaviour
{
	public static Inventory Instance;

	[Header("Prefabs")]
	public GameObject DescriptionPanelPrefab;
	
	[Header("References")]
	public GameObject ItemSlotInstance;
	public GameObject InventoryPanel;

	public static DescriptionPanel DescriptionPanel;


	private ItemSlot[] inventorySlots;
	public ItemSlot[] handSlots = new ItemSlot[2];
	public ItemSlot armorSlot;

	[Header("Inventory")]
	[SerializeField]
	private List<Item> items = new List<Item>();
	[SerializeField]
	private Weapon[] hands = new Weapon[2];
	[SerializeField]
	private Item armor = null;

	public int Size = 20; //max size of the inventory

	public int ItemCount { get => items.Count; }

	public Item this[int i]
	{
		get { return inventorySlots[i].Item; }
	}

	public void Awake()
	{
		Instance = this;
	}

	public void Start()
	{
		DescriptionPanel = Instantiate(DescriptionPanelPrefab, GameManager.Instance.FrontCanvas).GetComponentInChildren<DescriptionPanel>();
		DescriptionPanel.Hide();

		//create (size-1) slots, because the 1st one already exists
		for (int i = 1; i < Size; i++)
			Instantiate(ItemSlotInstance, InventoryPanel.transform);

		inventorySlots = InventoryPanel.GetComponentsInChildren<ItemSlot>();

		for (int i = 0; i < Size; i++)
		{
			if (i < items.Count) inventorySlots[i].SetItem(items[i]);
			else inventorySlots[i].SetItem(null);
		}
		handSlots[0].SetItem(hands[0]);
		handSlots[1].SetItem(hands[1]);
		armorSlot.SetItem(armor);

	}

	public bool Add(Item item)
	{
		if (ItemCount >= Size) return false;

		var newItem = Item.Instantiate(item);
		
		FindInSlots(null, out ItemSlot slot);

		items.Add(newItem);
		slot.SetItem(newItem);

		return true;
	}

	public bool Remove(Item item)
	{
		if (ItemCount == 0) return false;

		FindInSlots(item, out ItemSlot slot);

		if(!items.Remove(item)) return false;
		slot.SetItem(null);

		return true;
	}

	public void Swap(ItemSlot slot1, ItemSlot slot2)
	{
		var it = slot1.Item;
		slot1.SetItem(slot2.Item);
		slot2.SetItem(it);

	}

	public bool FindInSlots(Item item, out ItemSlot slot)
	{
		for(int i = 0; i < Size; ++i)
		{
			if (inventorySlots[i].Item == item)
			{
				slot = inventorySlots[i];
				return true;
			}
		}
		if(item == handSlots[(int)Hand.Right].Item)
		{
			slot = handSlots[0];
			return true;
		}
		else if (item == handSlots[(int)Hand.Left].Item)
		{
			slot = handSlots[1];
			return true;
		}else if (item == armorSlot.Item)
		{
			slot = armorSlot;
			return true;
		}

		slot = null;

		return false;
	}

	public bool EquipHand(Weapon weapon, Hand hand)
	{
		if (hands[(int)hand] != null)
		{
			
		}
		return false;
	}
}
