using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
	public static Inventory Instance;

	[Header("Prefabs")]
	public GameObject DescriptionPanelPrefab;
	
	[Header("References")]
	public GameObject ItemSlotInstance;
	public GameObject InventoryWindow;
	public GameObject InventoryPanel;
	public Button InventoryButton;
	public Text MoneyText;
	public GameObject MerchantWindow;


	private Vector3 InitialWindowPosition;

	public static DescriptionPanel DescriptionPanel;

	private List<ItemSlot> inventorySlots;

	[Header("Inventory")]
	[SerializeField]
	private List<Item> items = new List<Item>();

	private int money = 0;
	public int Money {
		get => money;
		set
		{
			money = value;
			MoneyText.text = $"Money : {money}";
		}
	}

	public bool CanSellItems = false;

	public List<Item> Items { get => items; }

	public int Size = 20; //max size of the inventory

	public int TotalItemCount { get => items.Count; }
	public int ItemInInventoryCount()
	{
		return inventorySlots.FindAll((slot) => (slot.Item != null)).Count;
	}

	#region Window
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

	public void OpenMerchantWindow(Merchant merchant)
	{
		MerchantWindow.gameObject.SetActive(true);
		MerchantWindow.GetComponent<MerchantPanel>().SetMerchant(merchant);
		GameManager.Instance.LockMap = true;
		CanSellItems = true;
	}

	public void CloseMerchantWindow()
	{
		MerchantWindow.gameObject.SetActive(false);
		GameManager.Instance.LockMap = false;
		CanSellItems = false;
	}

	#endregion

	public Item this[int i]
	{
		get { return inventorySlots[i].Item; }
	}

	public void Awake()
	{
		Instance = this;
		InitialWindowPosition = InventoryWindow.transform.position;
		Money = 0;
	}

	public void Start()
	{
		DescriptionPanel = Instantiate(DescriptionPanelPrefab, GameManager.Instance.FrontCanvas).GetComponentInChildren<DescriptionPanel>();
		DescriptionPanel.Hide();

		//create (size-1) slots, because the 1st one already exists
		for (int i = 1; i < Size; i++)
			Instantiate(ItemSlotInstance, InventoryPanel.transform);

		inventorySlots = new List<ItemSlot>(InventoryPanel.GetComponentsInChildren<ItemSlot>());

		for (int i = 0; i < Size; i++)
		{
			if (i < TotalItemCount) inventorySlots[i].SetItem(items[i]);
			else inventorySlots[i].SetItem(null);
			inventorySlots[i].AllowedItemType = typeof(Item);
		}
	}

	public bool Add(Item item)
	{
		if (item == null) return true;

		if (ItemInInventoryCount() >= Size) return false;

		var newItem = Item.Instantiate(item);
		
		FindInSlots(null, out ItemSlot slot);

		items.Add(newItem);
		slot.SetItem(newItem);

		return true;
	}

	public bool Remove(Item item)
	{
		if (item == null) return true;

		if (TotalItemCount == 0) return false;

		FindInSlots(item, out ItemSlot slot);

		if(!items.Remove(item)) return false;
		slot.SetItem(null);

		return true;
	}

	public bool Swap(ItemSlot from, ItemSlot to)
	{
		if (!to.CanSet(from.Item) || !to.CanBeSwapped)
			return false;

		var it = from.Item;
		from.SetItem(to.Item);
		to.SetItem(it);

		return true;
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

		slot = null;

		return false;
	}

}
