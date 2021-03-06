﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Fight
{
	// in
	public Team PlayerTeam;
	public Team EnemyTeam;
	public Unit CurrentActor;
	public List<CombatAction> CombatActions;

	//out
	public CombatAction CurrentCombatAction;
	public bool Escape = false;
	public int XP = 0;
	public List<Item> Loot = new List<Item>();

	public List<CombatAction> MakeEnemyActions()
	{
		var enemiesActions = new List<CombatAction>();
		var aliveTargets = PlayerTeam.Where(unit => !unit.IsDead);
		for (int i = 0; i < EnemyTeam.Count; ++i)
		{
			if (EnemyTeam[i].IsDead) continue;
			enemiesActions.Add(new CombatAction()
			{
				Actor = EnemyTeam[i],
				Type = CombatAction.ActionType.Attack,
				Targets = new List<Unit>() { aliveTargets.ElementAt(Random.Range(0, aliveTargets.Count())) }
			});
		}
		return enemiesActions;
	}



	//Prompts
	public void ChooseFightOrEscape(Prompt prompt)
	{
		GameManager.Instance.CreateText("Should you fight, or escape ?");

		GameManager.Instance.CreateButton("Fight",
			delegate {
				Escape = false;
				prompt.Proceed();
			});

		GameManager.Instance.CreateButton("Escape",
			delegate {
				Escape = true;
				prompt.Proceed();
			});
	}

	public void ChooseAction(Prompt prompt)
	{
		CurrentCombatAction = null;

		GameManager.Instance.CreateText($"What should {CurrentActor.Name} do ?");

		GameManager.Instance.CreateButton("Attack",
			delegate {
				CurrentCombatAction = new CombatAction()
				{
					Actor = CurrentActor,
					Type = CombatAction.ActionType.Attack
				};
				prompt.Next = new Prompt(ChooseTargets);
				prompt.Proceed();
			});

		var abilityButton = GameManager.Instance.CreateButton("Use ability",
			delegate
			{
				CurrentCombatAction = new CombatAction()
				{
					Actor = CurrentActor,
					Type = CombatAction.ActionType.Ability,
				};
				prompt.Next = new Prompt(ChooseAbility);
				prompt.Proceed();
			});
		abilityButton.interactable = (CurrentActor.Abilities.Count > 0);

		var itemButton = GameManager.Instance.CreateButton("Use Item",
			delegate {
				CurrentCombatAction = new CombatAction()
				{
					Actor = CurrentActor,
					Type = CombatAction.ActionType.UseItem
				};
				prompt.Next = new Prompt(ChooseItem);
				prompt.Proceed();
			});
		itemButton.interactable = (Inventory.Instance.Items.Any(item => item is Consumable));

		GameManager.Instance.CreateButton("Back",
			delegate {
				prompt.Proceed();
			});

	}

	public void ChooseAbility(Prompt prompt)
	{
		GameManager.Instance.CreateText($"Which ability should {CurrentActor.Name} use ?");

		foreach (Ability ability in CurrentActor.Abilities)
		{
			var button = GameManager.Instance.CreateButton($"{ability.Name} ({ability.FocusCost} focus)",
			delegate {
				CurrentCombatAction.Ability = ability;
				prompt.Next = new Prompt(ChooseTargets);
				prompt.Proceed();
			});
			button.interactable = (CurrentActor.Focus >= ability.FocusCost);
		}

		GameManager.Instance.CreateButton("Back",
		delegate {
			prompt.Next = new Prompt(ChooseAction);
			prompt.Proceed();
		});
	}

	public void ChooseItem(Prompt prompt)
	{
		GameManager.Instance.CreateText($"Which item should {CurrentActor.Name} use ?");

		foreach(Item item in Inventory.Instance.Items)
		{
			if (!(item is Consumable consumable)) continue;
			var button = GameManager.Instance.CreateButton(consumable.Name,
			delegate {
				CurrentCombatAction.Item = consumable;
				prompt.Next = new Prompt(ChooseTargets);
				prompt.Proceed();
			});
			button.interactable = !(CombatActions.Any(ca => (ca != null && ca.Item == consumable)));
		}

		GameManager.Instance.CreateButton("Back",
		delegate {
			prompt.Next = new Prompt(ChooseAction);
			prompt.Proceed();
		});

	}

	public void ChooseTargets(Prompt prompt)
	{
		Team parsableTeam;
		if ((CurrentCombatAction.Item != null && 
			CurrentCombatAction.Item.Type == Consumable.ConsumableType.Heal) ||
			(CurrentCombatAction.Ability != null &&
			CurrentCombatAction.Ability.AbilityType == AbilityType.Heal))
		{
			parsableTeam = PlayerTeam;
		}
		else
		{
			parsableTeam = EnemyTeam;
		}

		GameManager.Instance.CreateText($"Who should {CurrentActor.Name} target ?");

		if (CurrentCombatAction.Ability != null &&
			CurrentCombatAction.Ability.TargettingType == TargettingType.All)
		{
			GameManager.Instance.CreateButton("All",
				delegate
				{
					CurrentCombatAction.Targets = parsableTeam.Units;
					//end the chain
					prompt.Proceed();
				});
		}
		else
		{
			foreach (Unit unit in parsableTeam.Where(unit => !unit.IsDead))
			{
				GameManager.Instance.CreateButton(unit.Name,
					delegate
					{
						//set target
						CurrentCombatAction.Targets = new List<Unit>() { unit };

					//end the chain
					prompt.Proceed();
					});
			}
		}
		GameManager.Instance.CreateButton("Back",
			delegate {
				prompt.Next = new Prompt(ChooseAction);
				prompt.Proceed();
			});
	}
}
