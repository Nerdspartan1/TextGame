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

	[Header("Fight Style")]
	public Vector2Int damage;

	[Header("Drop")]
	public int xpDrop;
	public LootDrop[] lootDrops;

	public override void Attack(Unit other, out ActionResult result)
	{
		result = new ActionResult();
		result.IntValue = other.TakeDamage(Random.Range(damage.x, damage.y));
		result.Missed = false;
		result.Killed = other.IsDead;
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
