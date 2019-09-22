using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObjects/Unit/Enemy", order = 1)]
public class Enemy : Unit
{

	[System.Serializable]
	public struct LootDrop
	{
		public Item loot;
		[Range(0,1)]
		public float dropChance;
	}

	[Header("Drop")]
	public int xpDrop;
	public LootDrop[] lootDrops;

	public override void Attack(Unit target, out CombatAction.Result result)
	{
		result = new CombatAction.Result();
		result.IntValue = target.TakeDamage(Random.Range(Strength, Strength + Skill));
		result.Missed = false;

	}

	public List<Item> GetLoot()
	{
		List<Item> loot = new List<Item>();
		foreach (var lootDrop in lootDrops)
		{
			if (Random.value < lootDrop.dropChance)
				loot.Add(lootDrop.loot);
		}
		return loot;
	}

}
