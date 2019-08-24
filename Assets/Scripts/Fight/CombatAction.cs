using System.Collections.Generic;
using UnityEngine;

public class ActionResult
{
	public bool Missed = false;
	public int IntValue = 0;
	public bool Killed = false;
	public List<Item> Loot = new List<Item>();
	public int XP = 0;
}

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

	public void Execute(Fight fight)
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
				if (result.Killed) GameManager.Instance.CreateText($"{Target.Name} is K.O. !");
				fight.XP += result.XP;
				fight.Loot.AddRange(result.Loot);
				break;
			default: break;
		}
	}
}
