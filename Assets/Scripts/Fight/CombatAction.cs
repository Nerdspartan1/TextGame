using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class CombatAction
{
	public class Result
	{
		//action
		public bool Missed = false;
		public int IntValue = 0;
		//rewards
		public List<Item> Loot = new List<Item>();
		public int XP = 0;
	}

	public enum ActionType
	{
		Attack,
		UseItem,
		Ability,
	}

	public Unit Actor;
	public ActionType Type;

	public Consumable Item;
	public Ability Ability;

	public List<Unit> Targets;


	public void Execute(Fight fight)
	{
		if (Actor.IsDead)
		{
			Debug.LogWarning($"Cannot do action because actor '{Actor.Name}' is dead");
			return;
		}
		if (Targets.All(unit => unit.IsDead))
		{
			if (Targets.Count == 1)
			{
				if (Targets[0] is Character)
				{
					Targets = new List<Unit>() { GameManager.Instance.PlayerTeam.FirstOrDefault(unit => !unit.IsDead) };
				}
				else if (Targets[0] is Enemy)
				{
					Targets = new List<Unit>() { fight.EnemyTeam.FirstOrDefault(unit => !unit.IsDead) };
				}

				if (Targets[0] == null) return;

			}
			else if (Targets.Count > 1)
				return;
		}

		Result result = new Result();

		switch (Type)
		{
			case ActionType.Attack:
				Actor.Attack(Targets[0], out result);
				GameManager.Instance.CreateText($"{Actor.Name} attacks {Targets[0].Name} for {result.IntValue} damage !");
				break;
			case ActionType.Ability:
				Actor.UseAbility(Targets, Ability, out result);
				if (Targets.Count == 1)
					GameManager.Instance.CreateText($"{Actor.Name} uses {Ability.Name} on {Targets[0].Name}.");
				else
					GameManager.Instance.CreateText($"{Actor.Name} uses {Ability.Name}.");

				foreach(var target in Targets)
				{
					switch (Ability.AbilityType)
					{
						case AbilityType.Heal:
							GameManager.Instance.CreateText($"{target.Name} restores {result.IntValue} HP!");
							break;
						case AbilityType.Damage:
							GameManager.Instance.CreateText($"{target.Name} takes {result.IntValue} damage!");
							break;
					}
				}
				break;
			case ActionType.UseItem:
				Item.Use(Targets[0]);
				GameManager.Instance.CreateText($"{Actor.Name} uses {Item.Name} on {Targets[0].Name}.");
				break;
			default: break;
		}

		foreach (var target in Targets)
		{
			if (target.IsDead)
			{
				GameManager.Instance.CreateText($"{target.Name} is K.O. !");
				if (target is Enemy enemy)
				{
					result.XP += enemy.xpDrop;
					result.Loot.AddRange(enemy.GetLoot());
				}
			}
		}

		fight.XP += result.XP;
		fight.Loot.AddRange(result.Loot);
	}
}
