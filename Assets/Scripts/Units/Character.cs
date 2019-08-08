using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "ScriptableObjects/Unit/Character", order = 1)]
public class Character : Unit
{

	public int Xp;

	[Header("Weapon")]
	public Weapon Weapon;

	public override void Init()
	{
		base.Init();
		Xp = 0;
	}

	public override void Attack(Unit other, out ActionResult result)
	{
		result = new ActionResult();
		if (Weapon != null)
			result.IntValue = other.TakeDamage(Weapon.GetDamage());
		else
			result.IntValue = other.TakeDamage(1);

		result.Missed = false;
	}

	protected override void Die()
	{
		Debug.Log($"{Name} has died !");
	}
}
