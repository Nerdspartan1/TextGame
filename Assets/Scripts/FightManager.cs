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
	private Enemy foe;
	[SerializeField]
	private FightStatus fightStatus = FightStatus.Idle;


	public void BeginFight(Enemy foe)
	{
		Debug.Log("Fight start !");
		this.foe = foe;
		foe.Init();
		DoPlayerTurn();
	}


	private void DoPlayerTurn()
	{
		fightStatus = FightStatus.Idle;
		GenerateButtons();
		GameManager.Instance.UpdatePlayerInfo();
	}

	private void DoEnemyTurn()
	{
		StartCoroutine("EnemyAttackCoroutine");
	}

	private IEnumerator EnemyAttackCoroutine()
	{
		fightStatus = FightStatus.EnemyAttack;
		fightPanel.SetActive(true);
		fightMaskPanel.SetActive(true);

		//vvv Mettre ça dans le beginFight pour ne créer les unitTarget qu'une fois au début et les enlever à la fin
		GameObject unitTarget = Instantiate(unitTargetObject,fightPanel.transform);
		unitTarget.transform.localPosition = Vector3.zero;
		//unitTarget.GetComponent<UnitTarget>().unit = GameManager.Instance.player;


		yield return new WaitForSeconds(5);

		

		fightPanel.SetActive(false);
		fightMaskPanel.SetActive(false);

		if (!GameManager.Instance.player.IsDead)
		{
			DoPlayerTurn();
		}
		else
		{
			EndFight(false);
		}
	}

	void GenerateButtons()
	{
		GameManager.Instance.ClearButtons();
		GameObject go = GameObject.Instantiate(GameManager.Instance.buttonObject, GameManager.Instance.buttonPanel);
		go.GetComponentInChildren<Text>().text = "Attack";
		go.GetComponent<Button>().onClick.AddListener(PlayerAttack);
		go.GetComponent<Button>().onClick.AddListener(GameManager.Instance.ClearButtons);

	}

	void PlayerAttack()
	{
		fightStatus = FightStatus.PlayerAttack;
		Debug.Log("Vous attaquez l'ennemi !");
		GameManager.Instance.player.Attack(foe);

		if (!foe.IsDead)
		{
			DoEnemyTurn();
		}
		else
		{
			EndFight(true);
		}
	}

	void EndFight(bool victory)
	{
		if (victory)
		{
			GameManager.Instance.player.Xp += foe.xpDrop;
			Debug.Log("You Win !");
		}
		else
		{
			Debug.Log("Game Over !");
		}

		foe = null;
		Debug.Log("Fight end.");
	}

}
