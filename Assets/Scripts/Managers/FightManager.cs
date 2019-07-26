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

	[SerializeField]
	private Team EnemyTeam;

	private bool waitForInput = false;

	class CombatAction
	{
		public delegate void ActionDelegate(Unit Target);
		public ActionDelegate Action;
		public Unit Target;
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
		Dictionary<Unit, CombatAction> actions = new Dictionary<Unit, CombatAction>();

		//Player chooses strategy
		foreach(Unit teammate in GameManager.Instance.PlayerTeam)
		{
			yield return ChooseAction(teammate, actions);
			Debug.Log("Ok !");
		}

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
		List<Unit> order = GameManager.Instance.PlayerTeam.Units;
		order.AddRange(EnemyTeam);

		order.OrderByDescending(unit => unit.Speed);

		return order;
	}

	IEnumerator WaitForInput()
	{
		waitForInput = true;
		while (waitForInput) yield return null;
	}

	void InputReceived()
	{
		waitForInput = false;
	}

	IEnumerator ChooseAction(Unit teammate, Dictionary<Unit, CombatAction> actions)
	{
		CombatAction action = null;
		do
		{
			GameManager.Instance.CreateButton("Attack", InputReceived,
				delegate { action = new CombatAction() { Action = teammate.Attack }; });

			GameManager.Instance.CreateButton("Escape");

			yield return WaitForInput();

			GameManager.Instance.ClearButtons();

			if (action != null)
			{
				yield return DisplayTargets(action);

			}
			else
			{
				Debug.Log("Escape here");
			}

		} while (action?.Target == null);

	}

	IEnumerator DisplayTargets(CombatAction action)
	{
		foreach(Enemy enemy in EnemyTeam)
		{
			GameManager.Instance.CreateButton(enemy.Name, InputReceived, delegate {
				action.Target = enemy;
			});
		}
		GameManager.Instance.CreateButton("Back", InputReceived);

		yield return WaitForInput();

		GameManager.Instance.ClearButtons();

		if(action.Target != null)
		{
			Debug.Log($"Target is set to : {action.Target.Name}");
		}
		else
		{
			Debug.Log("Cancelled. Target is null, back to action choice");
			action = null;
		}

	}
}
