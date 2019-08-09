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

	class CombatAction
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
			if (Actor.IsDead || Target.IsDead) return;

			switch (Type)
			{
				case ActionType.Attack:
					Actor.Attack(Target,out ActionResult result);
					GameManager.Instance.CreateText($"{Actor.Name} attacks {Target.Name} for {result.IntValue} damage !");
					break;
				default: break;
			}
		}
	}

	class Prompt
	{

		public Prompt Next;
		public System.Action<CombatInfo, Prompt> Method;

		private bool waitForInput = false;

		public Prompt(System.Action<CombatInfo, Prompt> method)
		{
			Method = method;
		}

		IEnumerator WaitForInput()
		{
			waitForInput = true;
			while (waitForInput) yield return null;
		}

		public void Proceed()
		{
			waitForInput = false;
		}

		public IEnumerator Display(CombatInfo info = null)
		{
			//do thing
			Method.Invoke(info, this);

			yield return WaitForInput();

			GameManager.Instance.ClearButtons();

			if (Next == null) //end of prompt chain
				yield break;
			else
				yield return Next.Display(info);
		}
	}

	public Team PlayerTeam;
	public Team EnemyTeam;

	public void Start()
	{
		//we need to instantiate these so that we don't modify the source scriptable object
		PlayerTeam = Instantiate(PlayerTeam);
		PlayerTeam.InstantiateUnits();
	}

	public void BeginFight(Enemy enemy)
	{
		var enemyTeam = Team.CreateInstance<Team>();
		enemyTeam.Add(enemy);
		BeginFight(enemyTeam);
	}

	public void BeginFight(Team enemyTeam)
	{
		EnemyTeam = Instantiate(enemyTeam);
		EnemyTeam.InstantiateUnits();

		GameManager.Instance.ClearText();
		GameManager.Instance.HideMap = true;

		//placeholder for fight comment
		GameManager.Instance.CreateText($"The fight begins !");

		StartCoroutine(CombatLoopCoroutine());

	}

	public bool CheckFightOver(out bool playerVictory)
	{
		playerVictory = false;
		if (PlayerTeam.All(unit => unit.IsDead))
		{
			playerVictory = false;
			return true;
		}
		else if(EnemyTeam.All(unit => unit.IsDead))
		{
			playerVictory = true;
			return true;
		}
		return false;
	}

	class CombatInfo
	{
		public int CurrentTeammateId = 0;
		public CombatAction[] CombatActions;
	}

	IEnumerator CombatLoopCoroutine()
	{
		bool playerVictory;
		do
		{
			CombatInfo combatInfo = new CombatInfo();
			combatInfo.CombatActions = new CombatAction[PlayerTeam.Count + EnemyTeam.Count];

			//Fill the object with player choices
			yield return new Prompt(ChooseAction).Display(combatInfo);

			GameManager.Instance.ClearText();

			//Enemy AI strategy
			for (int i = 0; i < EnemyTeam.Count; ++i)
			{
				combatInfo.CombatActions[PlayerTeam.Count + i] = new CombatAction()
				{
					Actor = EnemyTeam[i],
					Type = CombatAction.ActionType.Attack,
					Target = PlayerTeam[Random.Range(0,PlayerTeam.Count)]
				};
			}

			//Build order by speed
			var orderedCombatActions = combatInfo.CombatActions.OrderByDescending(action => action.Actor.Speed);

			//Fight plays
			foreach (var action in orderedCombatActions)
			{
				action.Execute();
			}

			yield return new Prompt(PressOKToContinue).Display();

		} while (!CheckFightOver(out playerVictory));

		if(playerVictory) GameManager.Instance.CreateText("You win !");
		else GameManager.Instance.CreateText("You lose !");

	}

	private void DescribeStrategy(CombatInfo info)
	{
		for(int i = 0; i < PlayerTeam.Count; i++)
		{
			if (info.CombatActions[i] == null) continue;
			string actorName = PlayerTeam[i].Name;
			var action = info.CombatActions[i];
			string actionVerb = "???";
			switch (action.Type)
			{
				case CombatAction.ActionType.Attack:
					actionVerb = "attack";
					break;
				case CombatAction.ActionType.Heal:
					actionVerb = "heal";
					break;
			}
			GameManager.Instance.CreateText($"{actorName} will {actionVerb} {action.Target.Name}");
		}
	}

	void ChooseFightOrEscape(CombatInfo info, Prompt prompt)
	{
		GameManager.Instance.CreateText("Should you fight, or escape ?");

		GameManager.Instance.CreateButton("Fight",
			delegate {
				info.CurrentTeammateId = 0;
				prompt.Next = new Prompt(ChooseAction);
				prompt.Proceed();
			});

		GameManager.Instance.CreateButton("Escape",
			delegate {
				//Escape here
			});
	}

	void ChooseAction(CombatInfo info, Prompt prompt)
	{
		info.CombatActions[info.CurrentTeammateId] = null;

		GameManager.Instance.ClearText();
		DescribeStrategy(info);

		GameManager.Instance.CreateText($"What should {PlayerTeam[info.CurrentTeammateId].Name} do ?");

		GameManager.Instance.CreateButton("Attack",
			delegate {
				info.CombatActions[info.CurrentTeammateId] = new CombatAction() {
					Actor = PlayerTeam[info.CurrentTeammateId],
					Type = CombatAction.ActionType.Attack};
				prompt.Next = new Prompt(ChooseTargets);
				prompt.Proceed();
			});

		GameManager.Instance.CreateButton("Back",
			delegate {
				info.CurrentTeammateId--; // go to previous team member
				if (info.CurrentTeammateId >= 0)
					prompt.Next = new Prompt(ChooseAction);
				else
					prompt.Next = new Prompt(ChooseFightOrEscape);
				prompt.Proceed();
			});

	}

	void ChooseTargets(CombatInfo info, Prompt prompt)
	{
		GameManager.Instance.CreateText($"What should {PlayerTeam[info.CurrentTeammateId].Name} attack ?");

		foreach (Enemy enemy in EnemyTeam)
		{
			GameManager.Instance.CreateButton(enemy.Name,
				delegate
				{
					info.CombatActions[info.CurrentTeammateId++].Target = enemy; //set target and go to next teammate in team
				
					if(info.CurrentTeammateId < PlayerTeam.Count) //if there is a next teammate, make him choose action
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

	void PressOKToContinue(CombatInfo info, Prompt prompt)
	{
		GameManager.Instance.CreateButton("OK", 
			delegate {
				prompt.Proceed();
			});
	}
}
