using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;

public class ActionResult
{
	public bool Missed;
	public int IntValue;
}

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

	public void BeginFight(Enemy enemy)
	{
		var enemyTeam = Team.CreateInstance<Team>();
		enemyTeam.Add(enemy);
		BeginFight(enemyTeam);
	}

	public void BeginFight(Team enemyTeam)
	{
		Fight = new Fight();
		Fight.EnemyTeam = Instantiate(enemyTeam);
		Fight.EnemyTeam.InstantiateUnits();

		GameManager.Instance.RightPanel.gameObject.SetActive(true);
		EnemyTeamPanel.Team = Fight.EnemyTeam;
		EnemyTeamPanel.RebuildPanel();

		GameManager.Instance.ClearText();
		GameManager.Instance.HideMap = true;

		StartCoroutine(CombatLoopCoroutine());

	}

	private FightOutcome CheckFightOutcome()
	{
		if (GameManager.Instance.PlayerTeam.All(unit => unit.IsDead))
			return FightOutcome.Defeat;
		else if(Fight.EnemyTeam.All(unit => unit.IsDead))
			return FightOutcome.Victory;
		else
			return FightOutcome.NotFinished;
	}

	IEnumerator CombatLoopCoroutine()
	{
		FightOutcome outcome;
		do
		{
			Fight.ResetCombatActions();

			//Player Strategy
			yield return new Prompt(Fight.ChooseAction).Display();

			if (Fight.Escape)
			{
				outcome = FightOutcome.Escape;
				break;
			}

			GameManager.Instance.ClearText();

			//Enemy AI strategy
			Fight.MakeEnemyActions();

			//Build order by speed
			var orderedCombatActions = Fight.GetOrderedActions();

			//Fight plays
			foreach (var action in orderedCombatActions)
			{
				action.Execute();
			}

			EnemyTeamPanel.UpdateSlots();

			yield return new Prompt(Prompt.PressOKToContinue).Display();

			outcome = CheckFightOutcome();
		} while (outcome == FightOutcome.NotFinished);

		yield return EndFight(outcome);
	}

	private IEnumerator EndFight(FightOutcome outcome)
	{
		GameManager.Instance.RightPanel.gameObject.SetActive(false);
		switch (outcome)
		{
			case FightOutcome.Victory:
				GameManager.Instance.CreateText("You win !");
				//rewards
				break;
			case FightOutcome.Defeat:
				GameManager.Instance.CreateText("You lose !");
				//game over
				break;
			case FightOutcome.Escape:
				GameManager.Instance.CreateText("You escape successfully.");
				break;
		}

		yield return new Prompt(Prompt.PressOKToContinue).Display();

		GameManager.Instance.HideMap = false;
		//TODO: return to a chosen game event
		GameManager.Instance.DisplayParagraph(GameManager.Instance.CurrentMap[GameManager.Instance.CurrentLocation].description);
	}

}
