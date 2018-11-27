using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Weapon : Item {

	public int minDmg, maxDmg;

	public int GetDamage()
	{
		return (int)Random.Range(minDmg, maxDmg + 1);
	}

	override public void Load(string fileName)
	{
		StreamReader reader = new StreamReader(fileName);
		name = reader.ReadLine();
		desc = reader.ReadLine();
		minDmg = int.Parse(reader.ReadLine());
		maxDmg = int.Parse(reader.ReadLine());
	}
}
