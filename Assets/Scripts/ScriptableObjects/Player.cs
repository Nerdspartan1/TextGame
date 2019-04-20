using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "ScriptableObjects/Unit/Player", order = 1)]
public class Player : Unit {

	public static Player instance;

	public List<Item> inventory;
	private int xp = 0;

	public int Xp
	{
		get
		{
			return xp;
		}

		set
		{
			xp = value;
		}
	}

	public override void Init()
	{
		base.Init();
		Xp = 0;
		Level = 1;

		
	}
}
