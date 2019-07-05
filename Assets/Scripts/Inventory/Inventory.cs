using System.Collections.Generic;

public enum Hand
{
	Right,
	Left
}

public class Inventory
{

	private static Inventory instance;
	public static Inventory Instance
	{
		get
		{
			if (instance == null)
				instance = new Inventory();
			return instance;
		}
	}

	private readonly List<Item> items = new List<Item>(new Item[20]);

	public readonly Weapon[] hands = new Weapon[2];

	public int Size { get { return items.Count; } }

	private int itemCount = 0;

	public int ItemCount { get => itemCount; }

	public delegate void OnItemChanged(int i);
	public OnItemChanged onItemChanged;


	public Item this[int i]
	{
		get { return items[i]; }
	}

	public bool Add(Item item)
	{
		if (!Find(null, out int i)) return false;

		items[i] = Item.Instantiate(item);
		itemCount++;

		if (onItemChanged != null)
			onItemChanged.Invoke(i);

		return true;
	}

	public bool Remove(Item item)
	{
		if (!Find(item, out int i)) return false;

		items[i] = null;
		itemCount--;

		if (onItemChanged != null)
			onItemChanged.Invoke(i);

		return true;
	}

	public void Swap(int i1, int i2)
	{
		Item it = items[i1];
		items[i1] = items[i2];
		items[i2] = it;

		if (onItemChanged != null)
		{
			onItemChanged.Invoke(i1);
			onItemChanged.Invoke(i2);
		}

	}

	public bool Find(Item item, out int id)
	{
		for(id = 0; id < Size; ++id)
		{
			if (item == items[id]) return true;
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
