using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
	public static Inventory Instance;

	[Header("Prefabs")]
	public GameObject DescriptionPanelPrefab;
	
	[Header("References")]
	public GameObject ItemSlotInstance;
	public GameObject InventoryWindow;
	public GameObject InventoryPanel;

	private Vector3 InitialWindowPosition;

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
	private Armor armor = null;

	public int Size = 20; //max size of the inventory

	public int ItemCount { get => items.Count; }

	public void ToggleWindow()
	{
		InventoryWindow.gameObject.SetActive(!InventoryWindow.gameObject.activeInHierarchy);
		if (InventoryWindow.gameObject.activeInHierarchy)
			ResetWindowPosition();
	}

	public void ResetWindowPosition()
	{
		InventoryWindow.transform.position = InitialWindowPosition;
	}

	public Item this[int i]
	{
		get { return inventorySlots[i].Item; }
	}

	public void Awake()
	{
		Instance = this;
		InitialWindowPosition = InventoryWindow.transform.position;
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
			inventorySlots[i].AllowedItemType = typeof(Item);
		}
		handSlots[0].SetItem(hands[0]);
		handSlots[0].AllowedItemType = typeof(Weapon);
		handSlots[1].SetItem(hands[1]);
		handSlots[1].AllowedItemType = typeof(Weapon);
		armorSlot.SetItem(armor);
		armorSlot.AllowedItemType = typeof(Armor);

	}

	public bool Add(Item item)
	{
		if (item == null) return true;

		if (ItemCount >= Size) return false;

		var newItem = Item.Instantiate(item);
		
		FindInSlots(null, out ItemSlot slot);

		items.Add(newItem);
		slot.SetItem(newItem);

		return true;
	}

	public bool Remove(Item item)
	{
		if (item == null) return true;

		if (ItemCount == 0) return false;

		FindInSlots(item, out ItemSlot slot);

		if(!items.Remove(item)) return false;
		slot.SetItem(null);

		return true;
	}

	public bool Swap(ItemSlot from, ItemSlot to)
	{
		if (!to.CanSet(from.Item))
			return false;

		var it = from.Item;
		from.SetItem(to.Item);
		to.SetItem(it);

		UpdateEquipment();
		return true;
	}

	public void UpdateEquipment()
	{
		hands[0] = (Weapon)handSlots[0].Item;
		hands[1] = (Weapon)handSlots[1].Item;
		armor = (Armor)armorSlot.Item;
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
		if(item == handSlots[0].Item)
		{
			slot = handSlots[0];
			return true;
		}
		else if (item == handSlots[1].Item)
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

	public void EquipHand(Weapon weapon, int hand)
	{
		Remove(weapon);
		Add(hands[hand]);
		hands[hand] = weapon;
	}
}
