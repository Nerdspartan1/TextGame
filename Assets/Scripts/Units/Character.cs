using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "ScriptableObjects/Unit/Character", order = 1)]
public class Character : Unit
{
	public int XP = 0;

	public int AvailableAttributePoints;

	[Header("Weapon")]
	public Weapon Weapon;

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
		if(XP > XPLevel(Level+1))
		{
			Level++;
			AvailableAttributePoints++;
		}
	}

	public static int XPLevel(int level)
	{
		return (int)Mathf.Pow((level-1) * 4,2);
	}

	public void LevelUpAttribute(Attribute attribute)
	{
		switch (attribute)
		{
			case Attribute.Vitality: Vitality++; break;
			case Attribute.Strength: Strength++; break;
			case Attribute.Skill: Skill++; break;
			case Attribute.Endurance: Endurance++; break;
			case Attribute.Intelligence: Intelligence++; break;
			case Attribute.Speed: Speed++; break;
		}
		AvailableAttributePoints--;
		CalculateStatsFromAttributes();
	}
}
