using System.Collections.Generic;
using UnityEngine;

public enum Hand
{
	Right,
	Left
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
	[SerializeField]
	private List<Item> items = new List<Item>();

	private ItemSlot[] inventorySlots;

	public readonly Weapon[] hands = new Weapon[2];

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

	}

	public bool Add(Item item)
	{
		if (ItemCount >= Size) return false;

		var newItem = Item.Instantiate(item);
		
		FindInSlots(null, out int i);

		items.Add(newItem);
		inventorySlots[i].SetItem(newItem);

		return true;
	}

	public bool Remove(Item item)
	{
		if (ItemCount == 0) return false;

		FindInSlots(item, out int i);

		if(!items.Remove(item)) return false;
		inventorySlots[i].SetItem(null);

		return true;
	}

	public void Swap(int i1, int i2)
	{
		var it = inventorySlots[i1].Item;
		inventorySlots[i1].SetItem(inventorySlots[i2].Item);
		inventorySlots[i2].SetItem(it);

	}

	public bool FindInSlots(Item item, out int i)
	{
		for(i = 0; i < Size; ++i)
		{
			if (inventorySlots[i].Item == item) return true;
		}
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
