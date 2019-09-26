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
		{
			float damageMultiplier = 1 +
				Weapon.StrengthScale * ((Strength - Weapon.MinimumStrength) * 0.01f) +
				Weapon.SkillScale * ((Skill - Weapon.MinimumSkill) * 0.01f) +
				Weapon.IntelligenceScale * ((Intelligence - Weapon.MinimumIntelligence) * 0.01f);
			if (Strength < Weapon.MinimumStrength) damageMultiplier *= 0.5f;
			if (Skill < Weapon.MinimumSkill) damageMultiplier *= 0.5f;
			if (Intelligence < Weapon.MinimumIntelligence) damageMultiplier *= 0.5f;
			result.IntValue = target.TakeDamage((int)(damageMultiplier * Weapon.GetDamage()));
		}
		else
			result.IntValue = target.TakeDamage(Strength);
	}

	public void GainXP(int xp)
	{
		XP += xp;
		while(XP > LevelToXP(Level+1))
		{
			Level++;
			AvailableAttributePoints++;
		}
	}

	public static int LevelToXP(int level)
	{
		return 16 * (int)Mathf.Pow(level-1,2);
	}

	public static int XPToLevel(int xp)
	{
		return (int)(Mathf.Sqrt(xp)/4.0f) + 1;
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

	public override void CalculateStatsFromAttributes(bool reset = false)
	{
		base.CalculateStatsFromAttributes(reset);
		if (reset)
		{
			Level = (Strength + Skill + Intelligence) - 17;
			if (Level < 1) Level = 1;
			XP = LevelToXP(Level);
		}

	}
}
