using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "ScriptableObjects/Unit/Character", order = 1)]
public class Character : Unit
{
	public int XP = 0;

	public int AvailableAttributePoints;

	//[Header("Weapon")]
	[System.Xml.Serialization.XmlIgnore]
	public Weapon Weapon { get; private set; }

	//[HideInInspector]
	public bool CanBeRemovedFromFightTeam = true;
	public bool InFightTeam = true;

	public void Equip(Weapon weapon)
	{
		if (Weapon) Weapon.IsEquipped = false;
		Weapon = weapon;
		if (Weapon) Weapon.IsEquipped = true;
	}

	public override void Attack(Unit target, out CombatAction.Result result)
	{
		result = new CombatAction.Result();
		if (Weapon != null)
			result.IntValue = target.TakeDamage((int)(StrengthMultiplier * (float)Weapon.GetDamage()));
		else
			result.IntValue = target.TakeDamage(Strength);
	}

	public void GainXP(int xp)
	{
		XP += xp;
		while(XP > XPLevel(Level+1))
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
			case Attribute.Strength: Strength++; break;
			case Attribute.Skill: Skill++; break;
			case Attribute.Intelligence: Intelligence++; break;
		}
		AvailableAttributePoints--;
		CalculateStatsFromAttributes();
	}
}
