using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

	public void BeginFight(Enemy enemy)
	{
		var team = Team.CreateInstance<Team>();
		team.Units.Add(enemy);
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
		//Player chooses strategy
		foreach(Unit teammate in GameManager.Instance.PlayerTeam)
		{
			DisplayButtons(teammate);
			yield return WaitForInput();
			Debug.Log("Ok !");
		}


		//Fight plays

		
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

	void DisplayButtons(Unit teammate)
	{
		GameManager.Instance.CreateButton("Attack", InputReceived);
	}
	

}
