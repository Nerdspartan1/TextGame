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
	public int Vitality;
	public int Strength;
	public int Skill;
	public int Endurance;
	public int Intelligence;
	public int Speed;

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

	public void CalculateStatsFromAttributes()
	{
		MaxHp = (int)(20 + Vitality * 5);
	}

	public bool IsDead{ get => Hp <= 0; }

	public abstract void Attack(Unit other, out ActionResult result);

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
