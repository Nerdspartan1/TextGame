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
		public enum ActionType
		{
			Attack,
			Heal,
		}

		public delegate void ActionDelegate(Unit Target);
		public ActionType Type;
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

			if (Next == null)
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
		else
		{
			PlayerTeam = Instantiate(PlayerTeam);
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
		List<Unit> order = new List<Unit>();

		order.AddRange(PlayerTeam);
		order.AddRange(EnemyTeam);

		order.OrderByDescending(unit => unit.Speed);

		return order;
	}

	private void DescribeStrategy(StrategyInfo info)
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

	void ChooseFightOrEscape(StrategyInfo info, Prompt prompt)
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

	void ChooseAction(StrategyInfo info, Prompt prompt)
	{
		info.CombatActions[info.CurrentTeammateId] = null;

		GameManager.Instance.ClearText();
		DescribeStrategy(info);

		GameManager.Instance.CreateText($"What should {PlayerTeam[info.CurrentTeammateId].Name} do ?");

		GameManager.Instance.CreateButton("Attack",
			delegate {
				info.CombatActions[info.CurrentTeammateId] = new CombatAction() {
					Action = PlayerTeam[info.CurrentTeammateId].Attack,
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

	void ChooseTargets(StrategyInfo info, Prompt prompt)
	{
		GameManager.Instance.CreateText($"What should {PlayerTeam[info.CurrentTeammateId].Name} attack ?");

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
