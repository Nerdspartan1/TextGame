using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "ScriptableObjects/Unit/Unit", order = 1)]
public abstract class Unit : ScriptableObject
{
	[Header("Identity")]
	public string Name;
	[TextArea(3,10)]
	public string Description;

	[Header("Attributes")]
	public uint Strength;
	public uint Dexterity;
	public uint Perception;
	public uint Skill;
	public uint Endurance;

	[Header("Stats")]
	public uint Level;
	[SerializeField]
	private int maxHp;
	public int MaxHp
	{
		get { return maxHp; }
		set
		{
			maxHp = value;
			if (hp > maxHp) hp = maxHp;
		}
	}
	[SerializeField]
	private int hp;
	public int Hp
	{
		get { return hp; }
		set
		{
			hp = value;
			if (hp > maxHp) hp = maxHp;
		}

	}
	public float MoveSpeed;
	public float DamageMultiplier;


	public virtual void Init()
	{
		Hp = MaxHp;
		Debug.Log($"{Name} initialized.");
	}

	public void CalculateStatsFromAttributes()
	{
		MaxHp = (int)(20 + Endurance * 5 + Strength * 2);
	}

	public bool IsDead{ get { return Hp <= 0; }}

	public abstract void Attack(Unit other);

	public void TakeDamage(int dmg, out int dmgDone, out bool dies)
	{
		Hp -= dmg;
		dmgDone = dmg;
		dies = false;
		Debug.Log($"Damage inflicted to {Name} : {dmgDone}");
		if(Hp <= 0)
		{
			Die();
			dies = true;
		}
	}

	public void TakeDamage(int dmg)
	{
		int dmgDone;
		bool dies;
		TakeDamage(dmg,out dmgDone,out dies);
	}

	public void Die()
	{
		Debug.Log(Name + " has died !");
	}
}
