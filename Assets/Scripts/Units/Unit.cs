using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : ScriptableObject
{
	[Header("Identity")]
	public string Name;
	[TextArea(3,10)]
	public string Description;

	[Header("Attributes")]
	public int Strength;
	public int Speed;
	public int Perception;
	public int Skill;
	public int Endurance;

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

	public virtual void Init()
	{
		hp = MaxHp;
		Debug.Log($"{Name} initialized.");
	}

	public void CalculateStatsFromAttributes()
	{
		MaxHp = (int)(20 + Endurance * 5 + Strength * 2);
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

}
