using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : ScriptableObject
{
	[SerializeField]
	private string idName;

	[Header("Identity")]
	[SerializeField]
	new private string name;
	[SerializeField]
	[TextArea(3,10)]
	private string desc;
	
	[Header("Weapon")]
	public Weapon weapon;

	[Header("Stats")]
	public int STR; //+damage
	public int END; //+hp
	public int DEX; //+speed,dodges

	private int level = 1;
	private int hp, maxHp;

	public virtual void Init()
	{
		//Calcul des HP
		MaxHp = 10 + 1 * STR + 3 * END;
		Hp = MaxHp;
		Debug.Log(name + " initialized.\n Hp = " + Hp.ToString());
		
	}


	public int MaxHp
	{
		get
		{
			return maxHp;
		}

		set
		{
			maxHp = value;
			if (Hp > maxHp)
			{
				Hp = maxHp;
			}
			GameManager.ChangeValue(IdName + ".maxHp", maxHp);
		}
	}

	public int Hp
	{
		get
		{
			return hp;
		}

		set
		{
			hp = value;
			if(hp > MaxHp)
			{
				MaxHp = hp;
			}
			GameManager.ChangeValue(IdName + ".hp", hp);
		}
	}

	public bool IsDead
	{
		get { return Hp <= 0; }
	}

	public string Name
	{
		get
		{
			return name;
		}

		set
		{
			name = value;
			GameManager.ChangeName(IdName + ".name" , name);
		}
	}

	public string Desc
	{
		get
		{
			return desc;
		}

		set
		{
			desc = value;
			GameManager.ChangeName(IdName + ".desc", desc);
		}
	}

	public Weapon Weapon
	{
		get
		{
			return weapon;
		}

		set
		{
			weapon = value;
		}
	}

	public int Level
	{
		get
		{
			return level;
		}

		set
		{
			level = value;
			GameManager.ChangeValue(IdName + ".level", level);
		}
	}

	public string IdName
	{
		get
		{
			return idName;
		}

		set
		{
			idName = value;
		}
	}

	public void Attack(Unit other)
	{
		other.TakeDamage(Weapon.GetDamage());
	}

	public void TakeDamage(int dmg, out int dmgDone, out bool dies)
	{
		Hp -= dmg;
		dmgDone = dmg;
		dies = false;
		Debug.Log("Degâts infligés : " + dmgDone);
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
		Debug.Log(name + " dies !");

	}
}
