using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Fight{

	Player player;
	Unit foe;


	public Fight(Player player, Unit foe)
	{
		this.player = player;
		this.foe = foe;
	}

	public void Begin()
	{
		GameManager.Instance.mainText.text = "Le combat commence !";
		DoPlayerTurn();
	}

	public void DoTurn(bool playerTurn)
	{
		if (playerTurn)
		{
			DoPlayerTurn();
		}
		else
		{
			DoEnemyTurn();
		}
	}

	public void DoPlayerTurn()
	{
		GenerateButtons();
	}

	public void DoEnemyTurn()
	{
		foe.Attack(player);
		GameManager.Instance.mainText.text += "\n\n" + foe.name + " attaque " + player.name;
		DoPlayerTurn();
	}

	void GenerateButtons()
	{
		GameManager.Instance.ClearButtons();
		GameObject go = GameObject.Instantiate(GameManager.Instance.buttonObject, GameManager.Instance.buttonPanel);
		go.GetComponentInChildren<Text>().text = "Attaquer";
		go.GetComponent<Button>().onClick.AddListener(Attack);

	}

	void Attack()
	{
		player.Attack(foe);
		GameManager.Instance.mainText.text += "\n\n" + player.name + " attaque " + foe.name + " avec sa " + player.weapon.name;

		DoEnemyTurn();
	}
	
	void EndFight()
	{

	}


}
