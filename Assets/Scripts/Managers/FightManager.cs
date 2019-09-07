using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;

public class FightManager : MonoBehaviour
{
	//Singleton instance
	public static FightManager Instance;

	public void Awake()
	{
		Instance = this;
	}

	enum FightOutcome
	{
		NotFinished,
		Victory,
		Defeat,
		Escape
	}

	public TeamPanel EnemyTeamPanel;

	public Fight Fight;
	public GameEvent NextEvent;

	public void BeginFight(Enemy enemy, GameEvent NextEvent = null)
	{
		var enemyTeam = Team.CreateInstance<Team>();
		enemyTeam.Add(enemy);
		BeginFight(enemyTeam, NextEvent);
	}

	public void BeginFight(Team enemyTeam, GameEvent NextEvent = null, string introduction = "")
	{
		Fight = new Fight();
		Fight.EnemyTeam = Instantiate(enemyTeam);
		Fight.EnemyTeam.InstantiateUnits();
		Fight.PlayerTeam = new Team() { Units = GameManager.Instance.PlayerTeam.Units.FindAll(unit => (unit as Character).InFightTeam)};

		this.NextEvent = NextEvent;

		GameManager.Instance.RightPanel.gameObject.SetActive(true);
		EnemyTeamPanel.SetTeam(Fight.EnemyTeam);

		GameManager.Instance.ClearText();
		GameManager.Instance.LockMap = true;
		GameManager.Instance.LockInventory = true;
		GameManager.Instance.LockAbilities = true;
		GameManager.Instance.LockSave = true;


		GameManager.Instance.CreateText(introduction);

		StartCoroutine(CombatLoopCoroutine());
	}


	private FightOutcome CheckFightOutcome()
	{
		if (Fight.PlayerTeam.All(unit => unit.IsDead))
			return FightOutcome.Defeat;
		else if(Fight.EnemyTeam.All(unit => unit.IsDead))
			return FightOutcome.Victory;
		else
			return FightOutcome.NotFinished;
	}

	public IOrderedEnumerable<CombatAction> GetOrderedActions(IEnumerable<CombatAction> combatActions)
	{
		return combatActions.OrderByDescending(action => action.Actor.Speed);
	}

//	private void Describe(IEnumerable<CombatAction> actions)
//	{
//		foreach (var action in actions)
//		{
//			switch (action.Type)
//			{
//				case CombatAction.ActionType.Attack:
//					GameManager.Instance.CreateText($"{action.Actor.Name} will attack {action.Target.Name}.");
//					break;
//				case CombatAction.ActionType.UseItem:
//					GameManager.Instance.CreateText($"{action.Actor.Name} will use {action.Item.Name} on {action.Target.Name}.");
//					break;
//			}
//
//		}
//	}

	IEnumerator CombatLoopCoroutine()
	{
		
		yield return new Prompt(Fight.ChooseFightOrEscape).Display();
		FightOutcome outcome = Fight.Escape ? FightOutcome.Escape : FightOutcome.NotFinished;
		if (outcome == FightOutcome.Escape) goto FightEnd;

		do
		{
			//Player Strategy
			var alivePlayers = Fight.PlayerTeam.Where(unit => !unit.IsDead);
			Fight.CombatActions = new List<CombatAction>(new CombatAction[alivePlayers.Count()]);
			int teammateId = 0;
			while (teammateId < alivePlayers.Count()) // Fight or escape
			{
				while (teammateId < alivePlayers.Count()) // Choose actions
				{
					GameManager.Instance.ClearText();

					Fight.CurrentActor = alivePlayers.ElementAt(teammateId);
					yield return new Prompt(Fight.ChooseAction).Display();

					if (Fight.CurrentCombatAction != null)
					{
						Fight.CombatActions[teammateId++] = Fight.CurrentCombatAction;
					}
					else
					{
						if (teammateId == 0) break;
						Fight.CombatActions[--teammateId] = null;
					}
				}
				if (teammateId == 0)
				{
					GameManager.Instance.ClearText();

					yield return new Prompt(Fight.ChooseFightOrEscape).Display();
					if (Fight.Escape)
					{
						outcome = FightOutcome.Escape;
						goto FightEnd;
					}
				}
			}

			GameManager.Instance.ClearText();

			//Enemy AI strategy
			Fight.CombatActions.AddRange(Fight.MakeEnemyActions());

			//Build order by speed
			var orderedCombatActions = GetOrderedActions(Fight.CombatActions);

			//Fight plays
			foreach (var action in orderedCombatActions)
			{
				action.Execute(Fight);
			}

			EnemyTeamPanel.UpdateSlots();

			yield return new Prompt(Prompt.PressOKToContinue).Display();

			outcome = CheckFightOutcome();
		} while (outcome == FightOutcome.NotFinished);

		FightEnd: yield return EndFight(outcome);
	}

	private IEnumerator EndFight(FightOutcome outcome)
	{
		GameManager.Instance.RightPanel.gameObject.SetActive(false);

		if (outcome == FightOutcome.Defeat)
		{
			GameManager.Instance.CreateText("You lose ! Game over !");
		}
		else
		{
			if (outcome == FightOutcome.Victory) GameManager.Instance.CreateText("You win !");
			else GameManager.Instance.CreateText("You escape successfully.");

			if (Fight.XP > 0)
			{
				foreach (Character character in GameManager.Instance.PlayerTeam)
				{
					character.GainXP(Fight.XP);
					GameManager.Instance.CreateText($"{character.Name} gained {Fight.XP} XP.");
				}
			}
			if (Fight.Loot.Count > 0)
			{
				foreach (Item loot in Fight.Loot)
				{
					if (Inventory.Instance.Add(loot))
						GameManager.Instance.CreateText($"You received {loot.Name}.");
					else
						GameManager.Instance.CreateText($"You can't pick up {loot.Name} because your inventory is full.");
				}
			}

			yield return new Prompt(Prompt.PressOKToContinue).Display();

			GameManager.Instance.LockMap = false;
			GameManager.Instance.LockInventory = false;
			GameManager.Instance.LockAbilities = false;
			GameManager.Instance.LockSave = false;

			if (NextEvent) GameManager.Instance.PlayGameEvent(NextEvent);
			else GameManager.Instance.PlayGameEvent(GameManager.Instance.CurrentMap[GameManager.Instance.CurrentLocation]);
		}

	}

}
