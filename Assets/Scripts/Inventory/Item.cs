using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item/Item", order = 1)]
public class Item : ScriptableObject{
	
	public static Dictionary<string,Item> items = new Dictionary<string,Item>();
	
	[Header("Identity")]
	public string Name;
	public Sprite icon;
	[TextArea(3,10)]
	public string Description;
	public int Value;

	public virtual string Describe()
	{
		return Name + ":\n" + Description;
	}

}
