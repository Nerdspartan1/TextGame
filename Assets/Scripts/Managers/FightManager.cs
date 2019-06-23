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
	private Enemy Enemy;
	[SerializeField]
	private FightStatus fightStatus = FightStatus.Idle;

	public void BeginFight(Enemy foe)
	{
		Enemy = foe;
		Enemy.Init();
		GameManager.Instance.ClearText();
		GameManager.Instance.HideMap = true;

		//placeholder for fight comment
		GameManager.Instance.CreateText($"The fight between you and {Enemy.Name} has begun !");
		DoPlayerTurn();
	}


	private void DoPlayerTurn()
	{
		//placeholder for fight comment
		GameManager.Instance.CreateText($"Your turn!");
		fightStatus = FightStatus.Idle;
		GameManager.Instance.UpdatePlayerInfo();

		DisplayPlayerTurnButtons();
		
	}

	private void DoEnemyTurn()
	{
		//StartCoroutine("EnemyAttackCoroutine");
		//placeholder for fight comment
		GameManager.Instance.CreateText($"{Enemy.Name} attacks you !");
		Enemy.Attack(GameManager.Instance.Player);

		if (GameManager.Instance.Player.IsDead)
			EndFight(false);
		else
			DoPlayerTurn();
	}

	private IEnumerator EnemyAttackCoroutine()
	{
		fightStatus = FightStatus.EnemyAttack;
		fightPanel.SetActive(true);
		fightMaskPanel.SetActive(true);

		//Mettre ça dans le beginFight pour ne créer les unitTarget qu'une fois au début et les enlever à la fin
		GameObject unitTarget = Instantiate(unitTargetObject,fightPanel.transform);
		unitTarget.transform.localPosition = Vector3.zero;
		//unitTarget.GetComponent<UnitTarget>().unit = GameManager.Instance.player;


		yield return new WaitForSeconds(5);

		

		fightPanel.SetActive(false);
		fightMaskPanel.SetActive(false);

		if (!GameManager.Instance.Player.IsDead)
		{
			DoPlayerTurn();
		}
		else
		{
			EndFight(false);
		}
	}

	void DisplayPlayerTurnButtons()
	{
		GameManager.Instance.ClearButtons();
		GameManager.Instance.CreateButton("Attack", PlayerAttack);
	}

	void PlayerAttack()
	{
		fightStatus = FightStatus.PlayerAttack;
		//placeholder for fight comment
		GameManager.Instance.CreateText($"You attack {Enemy.Name} !");
		GameManager.Instance.Player.Attack(Enemy);

		if (Enemy.IsDead)
			EndFight(true);
		else
			DoEnemyTurn();
		
	}

	void EndFight(bool victory)
	{
		if (victory)
		{
			GameManager.Instance.Player.Xp += Enemy.xpDrop;
			//placeholder for fight comment
			GameManager.Instance.CreateText($"You defeated {Enemy.Name}!");
		}
		else
		{
			//placeholder for fight comment
			GameManager.Instance.CreateText($"You died !");

		}

		Enemy = null;
		GameManager.Instance.HideMap = false;
		Debug.Log("Fight end.");
	}

}
