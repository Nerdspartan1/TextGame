using System;
using UnityEngine;

public class CombatAction
{
	public enum ActionType
	{
		Attack,
		Heal,
	}

	public ActionType Type;
	public Unit Actor;
	public Unit Target;

	public void Execute()
	{
		if (Actor.IsDead || Target.IsDead)
		{
			if (Actor.IsDead) Debug.Log($"{Actor.Name} is dead");
			if (Target.IsDead) Debug.Log($"{Target.Name} is dead");
			return;
		}
		switch (Type)
		{
			case ActionType.Attack:
				Actor.Attack(Target, out ActionResult result);
				GameManager.Instance.CreateText($"{Actor.Name} attacks {Target.Name} for {result.IntValue} damage !");
				break;
			default: break;
		}
	}
}
