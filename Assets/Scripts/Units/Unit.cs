using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Attribute
{
	Vitality,
	Strength,
	Skill,
	Endurance,
	Intelligence,
	Speed,
}

public abstract class Unit : ScriptableObject
{
	[Header("Identity")]
	public string Name;
	[TextArea(3,10)]
	public string Description;

	[Header("Attributes")]
	public int Vitality = 10;
	public int Strength = 10;
	public int Skill = 10;
	public int Endurance = 10;
	public int Intelligence = 10;
	public int Speed = 10;

	[Header("Abilities")]
	public List<Ability> Abilities;

	[Header("Stats")]
	public int Level = 1;
	[SerializeField]
	private int maxHp;
	public int MaxHp {
		get => maxHp;
		set
		{
			maxHp = value;
			if (hp > maxHp) hp = maxHp;
		}
	}
	[SerializeField]
	private int hp;
	public int Hp {
		get => hp;
		set
		{
			hp = value;
			if (hp > maxHp) Debug.LogWarning("Hp is over MaxHp");
		}
	}

	public float StrengthMultiplier;
	public float DamageResistance;

	public void CalculateStatsFromAttributes()
	{
		MaxHp = Vitality < 40 ? (int)(8 + 300 * Mathf.Sin(Mathf.PI * Vitality / 100)) : 3 * Vitality + 173;
		StrengthMultiplier = 0.76f + 1.5f * Mathf.Sin((float)Strength * Mathf.PI / 200f);
	}

	public bool IsDead{ get => Hp <= 0; }

	public abstract void Attack(Unit target, out CombatAction.Result result);

	public void UseAbility(IEnumerable<Unit> targets, Ability ability, out CombatAction.Result result)
	{
		result = new CombatAction.Result();
		result.XP = 0;
		result.Loot = new List<Item>();
		foreach (var target in targets)
		{
			switch (ability.AbilityType)
			{
				case AbilityType.Heal:
					target.Heal(ability.Value);
					result.IntValue = ability.Value;
					break;
				case AbilityType.Damage:
					target.TakeDamage(ability.Value);
					result.IntValue = ability.Value;
					break;
			}
			
			result.Missed = false;
			if (target is Enemy enemy && enemy.IsDead)
			{
				result.XP = enemy.xpDrop;
				result.Loot = enemy.GetLoot();
			}

			if (ability.TargettingType == TargettingType.Single) break;
		}
	}

	public int TakeDamage(int dmg)
	{
		hp -= dmg;

		if(hp <= 0)
			hp = 0;


		return dmg;
	}

	public void Heal(int heal)
	{
		hp += heal;
		if(hp > MaxHp)
		{
			hp = MaxHp;
		}
	}

	public int GetAttribute(Attribute attribute)
	{
		switch (attribute)
		{
			case Attribute.Vitality: return Vitality;
			case Attribute.Strength: return Strength;
			case Attribute.Skill: return Skill;
			case Attribute.Endurance: return Endurance;
			case Attribute.Intelligence: return Intelligence;
			case Attribute.Speed: return Speed;
		}
		throw new System.Exception("Unknown attribute");
	}

}
