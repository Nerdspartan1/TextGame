using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObjects/Unit/Enemy", order = 1)]
public class Enemy : Unit
{

	[System.Serializable]
	public class LootDrop
	{
		public Item loot;
		public float dropChance;
	}

	[Header("Fight Style")]
	public Vector2Int damage;

	[Header("Drop")]
	public int xpDrop;
	public LootDrop[] lootDrops;

	public override void Attack(Unit other)
	{
		other.TakeDamage(Random.Range(damage.x, damage.y));
	}

}
