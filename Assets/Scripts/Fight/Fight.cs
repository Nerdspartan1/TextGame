using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Fight
{

	public Team EnemyTeam;

	private int CurrentTeammateId = 0;

	public CombatAction[] CombatActions;
	public bool Escape = false;

	public int XP = 0;
	public List<Item> Loot = new List<Item>();

	public void ResetCombatActions()
	{
		CurrentTeammateId = 0;
		CombatActions = new CombatAction[GameManager.Instance.PlayerTeam.Count + EnemyTeam.Count];
	}

	public void MakeEnemyActions()
	{
		for (int i = 0; i < EnemyTeam.Count; ++i)
		{
			CombatActions[GameManager.Instance.PlayerTeam.Count + i] = new CombatAction()
			{
				Actor = EnemyTeam[i],
				Type = CombatAction.ActionType.Attack,
				Target = GameManager.Instance.PlayerTeam[Random.Range(0, GameManager.Instance.PlayerTeam.Count)]
			};
		}
	}

	public IOrderedEnumerable<CombatAction> GetOrderedActions()
	{
		return CombatActions.OrderByDescending(action => action.Actor.Speed);
	}

	private void Describe()
	{
		for (int i = 0; i < GameManager.Instance.PlayerTeam.Count; i++)
		{
			if (CombatActions[i] == null) continue;
			string actorName = GameManager.Instance.PlayerTeam[i].Name;
			var action = CombatActions[i];
			switch (action.Type)
			{
				case CombatAction.ActionType.Attack:
					GameManager.Instance.CreateText($"{actorName} will attack {action.Target.Name}.");
					break;
				case CombatAction.ActionType.Heal:
					GameManager.Instance.CreateText($"{actorName} will heal {action.Target.Name}.");
					break;
				case CombatAction.ActionType.UseItem:
					GameManager.Instance.CreateText($"{actorName} will use {action.Item.Name} on {action.Target.Name}.");
					break;
			}
			
		}
	}

	//Prompts
	public void ChooseFightOrEscape(Prompt prompt)
	{
		GameManager.Instance.CreateText("Should you fight, or escape ?");

		GameManager.Instance.CreateButton("Fight",
			delegate {
				CurrentTeammateId = 0;
				prompt.Next = new Prompt(ChooseAction);
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
		CombatActions[CurrentTeammateId] = null;

		GameManager.Instance.ClearText();
		Describe();

		GameManager.Instance.CreateText($"What should {GameManager.Instance.PlayerTeam[CurrentTeammateId].Name} do ?");

		GameManager.Instance.CreateButton("Attack",
			delegate {
				CombatActions[CurrentTeammateId] = new CombatAction()
				{
					Actor = GameManager.Instance.PlayerTeam[CurrentTeammateId],
					Type = CombatAction.ActionType.Attack
				};
				prompt.Next = new Prompt(ChooseTargets);
				prompt.Proceed();
			});

		GameManager.Instance.CreateButton("Use Item",
			delegate {
				CombatActions[CurrentTeammateId] = new CombatAction()
				{
					Actor = GameManager.Instance.PlayerTeam[CurrentTeammateId],
					Type = CombatAction.ActionType.UseItem
				};
				prompt.Next = new Prompt(ChooseItem);
				prompt.Proceed();
			});

		GameManager.Instance.CreateButton("Back",
			delegate {
				CurrentTeammateId--; // go to previous team member
					if (CurrentTeammateId >= 0)
					prompt.Next = new Prompt(ChooseAction);
				else
					prompt.Next = new Prompt(ChooseFightOrEscape);
				prompt.Proceed();
			});

	}

	public void ChooseItem(Prompt prompt)
	{
		GameManager.Instance.CreateText($"Which item should {CombatActions[CurrentTeammateId].Actor.Name} use ?");

		for(int i = 0; i < Inventory.Instance.TotalItemCount; i++)
		{
			if(Inventory.Instance[i] is Consumable consumable)
			{
				GameManager.Instance.CreateButton($"Use {consumable.Name}",
				delegate {
					CombatActions[CurrentTeammateId].Item = consumable;
					prompt.Next = new Prompt(ChooseTargets);
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

	public void ChooseTargets(Prompt prompt)
	{
		Team parsableTeam;
		if (CombatActions[CurrentTeammateId].Type == CombatAction.ActionType.UseItem &&
			CombatActions[CurrentTeammateId].Item.Type == Consumable.ConsumableType.Heal)
		{
			
			parsableTeam = GameManager.Instance.PlayerTeam;
		}
		else
		{
			GameManager.Instance.CreateText($"What should {GameManager.Instance.PlayerTeam[CurrentTeammateId].Name} attack ?");
			parsableTeam = EnemyTeam;
		}

		foreach (Unit unit in parsableTeam)
		{
			GameManager.Instance.CreateButton(unit.Name,
				delegate
				{
					CombatActions[CurrentTeammateId++].Target = unit; //set target and go to next teammate in team

						if (CurrentTeammateId < GameManager.Instance.PlayerTeam.Count) //if there is a next teammate, make him choose action
							prompt.Next = new Prompt(ChooseAction);
						// else end the chain
						prompt.Proceed();
				});
		}
		GameManager.Instance.CreateButton("Back",
			delegate {
				prompt.Next = new Prompt(ChooseAction);
				prompt.Proceed();
			});
	}
}
