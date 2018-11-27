using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit{

	public int level;
	public int hp, maxHp;
	public string name;
	public string desc;
	public bool isDead;
	public Weapon weapon;

	public int STR, PER, END, CHR, INT, AGI, LCK;

	
	public void Attack(Unit other)
	{
		other.TakeDamage(weapon.GetDamage());
	}

	public void TakeDamage(int dmg, out int dmgDone, out bool dies)
	{
		hp -= dmg;
		dmgDone = dmg;
		dies = false;
		if(hp <= 0)
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
		isDead = true;
	}
}
