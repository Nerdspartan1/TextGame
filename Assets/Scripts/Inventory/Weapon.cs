using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/Item/Weapon", order = 1)]
public class Weapon : Item {

	[Header("Damage")]
	public int minDmg;
	public int maxDmg;

	public int GetDamage()
	{
		return Random.Range(minDmg, maxDmg + 1);
	}

}
