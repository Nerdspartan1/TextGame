using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "ScriptableObjects/Unit/Character", order = 1)]
public class Character : Unit
{

	public int XP = 0;

	[Header("Weapon")]
	public Weapon Weapon;

	public override void Init()
	{
		base.Init();
		XP = 0;
	}

	public override void Attack(Unit other, out ActionResult result)
	{
		result = new ActionResult();
		if (Weapon != null)
			result.IntValue = other.TakeDamage(Weapon.GetDamage());
		else
			result.IntValue = other.TakeDamage(1);

		result.Missed = false;
		result.Killed = other.IsDead;
		if (result.Killed) {
			result.XP = (other as Enemy).xpDrop;
			result.Loot = (other as Enemy).GetLoot();
		}
	}

	public void GainXP(int xp)
	{
		XP += xp;
		Level = (int)Mathf.Sqrt(1+(float)XP/4.0f);
	}
}
