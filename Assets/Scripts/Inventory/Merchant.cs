using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Merchant", menuName = "ScriptableObjects/Merchant")]
public class Merchant : ScriptableObject
{
	public string Name;
	public List<Item> Items;
}
