using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : ScriptableObject
{
	[SerializeField]
	public string Id;

	[Header("Identity")]
	public string Name;
	[TextArea(3,10)]
	public string Description;
	
	[Header("Weapon")]
	public Weapon Weapon;

	[Header("Stats")]
	public int STR; //+damage
	public int END; //+hp
	public int DEX; //+speed,dodges

	public int Level = 1;
	private int hp, maxHp;

	public virtual void Init()
	{
		MaxHp = 10 + 1 * STR + 3 * END;
		Hp = MaxHp;
		Debug.Log($"{Id} initialized.");
	}

	public int MaxHp
	{
		get{return maxHp;}
		set{
			maxHp = value;
			if (Hp > maxHp) Hp = maxHp;
		}
	}

	public int Hp
	{
		get{return hp;}
		set{
			hp = value;
			if(hp > MaxHp) MaxHp = hp;
		}
	}

	public bool IsDead{ get { return Hp <= 0; }}

	public void Attack(Unit other)
	{
		other.TakeDamage(Weapon.GetDamage());
	}

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
