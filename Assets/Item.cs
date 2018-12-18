using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Item{

	public static Dictionary<string,Item> items = new Dictionary<string,Item>();

	public string name;
	public string desc;
	public int value;

	public virtual void Load(string fileName)
	{
		StreamReader reader = new StreamReader(fileName);
		name = reader.ReadLine();
		desc = reader.ReadLine();

		value = int.Parse(reader.ReadLine());

	}

	public static void LoadItems(string path)
	{
		foreach (string file in Directory.GetFiles(path,"*.txt"))
		{
			Item i = new Item();
			i.Load(file);
			items.Add(GameManager.ExtractFileName(file), i);
		}
		foreach (string file in Directory.GetFiles(path + "/Weapons", "*.txt"))
		{
			Weapon w = new Weapon();
			w.Load(file);
			items.Add(GameManager.ExtractFileName(file), w);
		}
	}

	public virtual string Describe()
	{
		return  name + ":\n" + desc;
	}

	
}
