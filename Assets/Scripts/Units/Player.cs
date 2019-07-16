using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "ScriptableObjects/Unit/Player", order = 1)]
public class Player : Unit {

	public int Xp;

	[Header("Weapon")]
	public Weapon Weapon;

	public override void Init()
	{
		base.Init();
		Xp = 0;

	}

	public override void Attack(Unit other)
	{
		if (Weapon != null)
			other.TakeDamage(Weapon.GetDamage());
		else
			other.TakeDamage(1);
	}

	public override void Die()
	{
		Debug.Log("You died ! Game Over");
	}

}
