using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	public readonly List<Item> items = new List<Item>();

	public int Size = 20;

	public delegate void OnItemChanged();
	public OnItemChanged onItemChanged;

	public bool Add(Item item)
	{
		if (items.Count >= Size) return false;

		items.Add(item);

		if (onItemChanged != null)
			onItemChanged.Invoke();

		return true;
	}

	public bool Remove(Item item)
	{

		if (!items.Remove(item)) return false;
		
		if (onItemChanged != null)
			onItemChanged.Invoke();

		return true;

	}
}
