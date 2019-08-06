using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;

public enum FightStatus
{
	EnemyAttack,
	PlayerAttack,
	Idle,
}

public class FightManager : MonoBehaviour
{
	public static float fightSceneWidth = 600;
	public static float fightSceneHeight = 600;

	public GameObject fightPanel;
	public GameObject fightMaskPanel;
	public GameObject unitTargetObject;

	public Team PlayerTeam;
	[SerializeField]
	private Team EnemyTeam;

	class CombatAction
	{
		public delegate void ActionDelegate(Unit Target);
		public ActionDelegate Action;
		public Unit Target;
	}
	
	class Prompt
	{
		public Prompt Next;
		public Prompt(System.Action<StrategyInfo, Prompt> method)
		{
			Method = method;
		}

		private bool waitForInput = false;

		public System.Action<StrategyInfo, Prompt> Method;

		IEnumerator WaitForInput()
		{
			waitForInput = true;
			while (waitForInput) yield return null;
		}
		public void Proceed()
		{
			waitForInput = false;
		}

		public IEnumerator Execute(StrategyInfo info)
		{
			//do thing
			Method.Invoke(info, this);

			yield return WaitForInput();

			GameManager.Instance.ClearButtons();

			if(Next == null)
				throw new System.Exception("Next has not been set");

			if (Next.Method == null) //end of prompt chain
				yield break;
			else
				yield return Next.Execute(info);
		}
	}

	class StrategyInfo
	{
		public int CurrentTeammateId = 0;
		public CombatAction[] CombatActions;
	}

	public void Start()
	{
		if (PlayerTeam == null)
		{
			PlayerTeam = ScriptableObject.CreateInstance<Team>();
			PlayerTeam.Units.Add(GameManager.Instance.Player);
		}
	}

	public void BeginFight(Enemy enemy)
	{
		var team = Team.CreateInstance<Team>();
		team.Add(enemy);
		BeginFight(team);
	}
	public void BeginFight(Team enemies)
	{
		foreach(var enemy in enemies)
			enemy.Init();
		EnemyTeam = enemies;

		GameManager.Instance.ClearText();
		GameManager.Instance.HideMap = true;

		//placeholder for fight comment
		GameManager.Instance.CreateText($"The fight begins !");

		StartCoroutine(CombatLoopCoroutine());

	}

	IEnumerator CombatLoopCoroutine()
	{
		StrategyInfo strategyInfo = new StrategyInfo();
		strategyInfo.CombatActions = new CombatAction[PlayerTeam.Count];

		//Fill the object with player choices
		yield return new Prompt(ChooseFightOrEscape).Execute(strategyInfo);

		//Enemy AI strategy
		foreach(Enemy enemy in EnemyTeam)
		{

		}

		//Build order by speed
		List<Unit> order = BuildOrder();

		//Fight plays
		foreach(var unit in order)
		{

		}
		
	}

	private List<Unit> BuildOrder()
	{
		List<Unit> order = PlayerTeam.Units;
		order.AddRange(EnemyTeam);

		order.OrderByDescending(unit => unit.Speed);

		return order;
	}

	void ChooseFightOrEscape(StrategyInfo info, Prompt prompt)
	{
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

	void ChooseAction(StrategyInfo info, Prompt prompt)
	{
		GameManager.Instance.CreateButton("Attack",
			delegate {
				info.CombatActions[info.CurrentTeammateId] = new CombatAction() { Action = PlayerTeam[info.CurrentTeammateId].Attack };
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

	void ChooseTargets(StrategyInfo info, Prompt prompt)
	{
		foreach (Enemy enemy in EnemyTeam)
		{
			GameManager.Instance.CreateButton(enemy.Name,
				delegate
				{
					info.CombatActions[info.CurrentTeammateId++].Target = enemy; //set target and go to next teammate in team
				
					if(info.CurrentTeammateId < info.CombatActions.Length) //if there is a next teammate, make him choose action
						prompt.Next = new Prompt(ChooseAction);
					else
						prompt.Next = new Prompt(null); // else end the chain
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
