using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Attribute
{
	Strength,
	Skill,
	Intelligence,
}

public abstract class Unit : ScriptableObject
{
	[Header("Identity")]
	public string Name;
	[TextArea(3,10)]
	public string Description;

	[Header("Attributes")]
	public int Strength = 10;
	public int Skill = 10;
	public int Intelligence = 10;

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
	[SerializeField]
	private int maxFocus;
	public int MaxFocus
	{
		get => maxFocus;
		set
		{
			maxFocus = value;
			if (focus > maxFocus) focus = maxFocus;
		}
	}
	[SerializeField]
	private int focus;
	public int Focus
	{
		get => focus;
		set
		{
			focus = value;
			if (focus > maxFocus) Debug.LogWarning("Focus is over MaxFocus");
		}
	}

	public float StrengthMultiplier;
	public float SkillMultiplier;
	public float IntelligenceMultiplier;
	public float DamageResistance;

	public void CalculateStatsFromAttributes(bool reset = false)
	{
		int previousMaxHp = MaxHp;
		MaxHp = 20 + (int)(3.0f * (0.6f*Strength + 0.3f*Skill + 0.1f*Intelligence));
		Hp = reset ? MaxHp : Hp + (MaxHp - previousMaxHp);
		int previousMaxFocus = MaxFocus;
		MaxFocus = 5 * Intelligence;
		Focus = reset ? MaxFocus : Focus + (MaxFocus - previousMaxFocus);
		StrengthMultiplier = 0.90f + 0.01f * Strength;
		SkillMultiplier = 0.90f + 0.01f * Skill;
		IntelligenceMultiplier = 0.90f + 0.01f * Intelligence;
		
	}

	public bool IsDead{ get => Hp <= 0; }

	public abstract void Attack(Unit target, out CombatAction.Result result);

	public void UseAbility(IEnumerable<Unit> targets, Ability ability, out CombatAction.Result result)
	{
		if (ability.FocusCost > Focus) Debug.LogError("Cannot use ability : too low focus");

		Focus -= ability.FocusCost;

		result = new CombatAction.Result();
		foreach (var target in targets)
		{
			int multipliedValue = (int)(IntelligenceMultiplier * ability.Value);
			switch (ability.AbilityType)
			{
				case AbilityType.Heal:
					target.Heal(multipliedValue);
					result.IntValue = multipliedValue;
					break;
				case AbilityType.Damage:
					target.TakeDamage(multipliedValue);
					result.IntValue = multipliedValue;
					break;
			}
			
			result.Missed = false;

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
			case Attribute.Strength: return Strength;
			case Attribute.Skill: return Skill;
			case Attribute.Intelligence: return Intelligence;
		}
		throw new System.Exception("Unknown attribute");
	}

}
