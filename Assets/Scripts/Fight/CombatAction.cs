using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
		UseItem,
		Heal,
	}

	public ActionType Type;
	public Unit Actor;
	public Unit Target;
	public Consumable Item;

	public void Execute(Fight fight)
	{
		if (Actor.IsDead)
		{
			Debug.LogWarning($"Cannot do action because actor '{Actor.Name}' is dead");
			return;
		}
		if (Target.IsDead)
		{
			if(Target is Character)
			{
				Target = GameManager.Instance.PlayerTeam.FirstOrDefault(unit => !unit.IsDead);
			}
			else if (Target is Enemy)
			{
				Target = fight.EnemyTeam.FirstOrDefault(unit => !unit.IsDead);
			}

			if (!Target) return;
			
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
			case ActionType.UseItem:
				Item.Use(Target);
				GameManager.Instance.CreateText($"{Actor.Name} uses {Item.Name} on {Target.Name}.");
				break;
			default: break;
		}
	}
}
