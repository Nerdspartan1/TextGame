using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item/Item", order = 1)]
public class Item : ScriptableObject{
	
	public static Dictionary<string,Item> items = new Dictionary<string,Item>();
	
	[Header("Identity")]
	new public string name;
	[TextArea(3,10)]
	public string desc;
	public int value;

	public virtual string Describe()
	{
		return name + ":\n" + desc;
	}

	//Deprecated
	/*
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
	*/
}
