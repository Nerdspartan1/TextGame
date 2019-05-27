using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "ScriptableObjects/Unit/Player", order = 1)]
public class Player : Unit {

	public List<Item> inventory;
	public int Xp;

	[Header("Stats")]
	public uint Vitality;
	public uint Endurance;
	public uint Strength; 
	public uint Skill;
	public uint Luck;


	public override void Init()
	{
		base.Init();
		Xp = 0;
		Level = 1;
		CalculateStatsFromCharacteristics();
	}

	public void CalculateStatsFromCharacteristics()
	{
		uint V = Vitality;
		uint E = Endurance;
		uint St = Strength;
		uint Sk = Skill;
		uint L = Luck;
		
		MaxHp = (int)(20 + 5 * V + 2 * E);
		Hp = MaxHp;

	}
}
