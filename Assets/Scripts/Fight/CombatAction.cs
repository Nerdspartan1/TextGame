using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class CombatAction
{
	public class Result
	{
		public bool Missed = false;
		public int IntValue = 0;
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

	public Unit Target;


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

		Result result;

		switch (Type)
		{
			case ActionType.Attack:
				Actor.Attack(Target, out result);
				GameManager.Instance.CreateText($"{Actor.Name} attacks {Target.Name} for {result.IntValue} damage !");
				break;
			case ActionType.Ability:
				Actor.UseAbility(Target, Ability, out result);
				GameManager.Instance.CreateText($"{Actor.Name} uses {Ability.Name} on {Target.Name}.");
				switch (Ability.Type)
				{
					case Ability.AbilityType.Heal:
						GameManager.Instance.CreateText($"{Target.Name} restores {result.IntValue} HP!");
						break;
					case Ability.AbilityType.Damage:
						GameManager.Instance.CreateText($"{Target.Name} takes {result.IntValue} damage!");
						break;
				}
				if (Target.IsDead) GameManager.Instance.CreateText($"{Target.Name} is K.O. !");
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
