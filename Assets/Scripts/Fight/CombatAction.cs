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

		var aliveTargets = Targets.FindAll(unit => !unit.IsDead);
		if (aliveTargets.Count() == 0)
		{
			if (Targets.Count == 1) //select another alive target if available
			{
				if (Targets[0] is Character)
				{
					aliveTargets = new List<Unit>() { GameManager.Instance.PlayerTeam.FirstOrDefault(unit => !unit.IsDead) };
				}
				else if (Targets[0] is Enemy)
				{
					aliveTargets = new List<Unit>() { fight.EnemyTeam.FirstOrDefault(unit => !unit.IsDead) };
				}

				if (aliveTargets.First() == null) return;

			}
			else
				return;
		}

		Result result = new Result();

		switch (Type)
		{
			case ActionType.Attack:
				Actor.Attack(aliveTargets.First(), out result);
				GameManager.Instance.CreateText($"{Actor.Name} attacks {aliveTargets.First().Name} for {result.IntValue} damage !");
				break;
			case ActionType.Ability:
				Actor.UseAbility(aliveTargets, Ability, out result);
				if (aliveTargets.Count() == 1)
					GameManager.Instance.CreateText($"{Actor.Name} uses {Ability.Name} on {aliveTargets.First().Name}.");
				else
					GameManager.Instance.CreateText($"{Actor.Name} uses {Ability.Name}.");

				foreach(var target in aliveTargets)
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
				Item.Use(aliveTargets.First());
				GameManager.Instance.CreateText($"{Actor.Name} uses {Item.Name} on {aliveTargets.First().Name}.");
				break;
			default: break;
		}

		foreach (var target in aliveTargets)
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
