using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "ScriptableObjects/Unit/Player", order = 1)]
public class Player : Unit {

	public List<Item> inventory;
	public int Xp;


	public override void Init()
	{
		base.Init();
		Xp = 0;

	}

}
