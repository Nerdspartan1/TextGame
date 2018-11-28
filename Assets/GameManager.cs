using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	private static GameManager instance = null;

	static public Dictionary<string, string> names = new Dictionary<string, string>();
	static public Dictionary<string, float> values = new Dictionary<string, float>();

	public Transform textPanel;
	public Transform buttonPanel;
	public GameObject buttonObject;

	public string startGameEvent;

	public GameObject textBox;
	public GameObject dialogueBox;
	public Fight currentFight;
	public GameEvent currentGameEvent;

	bool buttonsDisplayed = false;

	void Start () {
		if(instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(this);
		}

		GameEvent.textBox = textBox;
		GameEvent.dialogueBox = dialogueBox;

		names.Add("name", "Maurice");
		values.Add("isHappy", 1);
		values.Add("isBored", 1);
		
		GameEvent ge = new GameEvent();
		ge.fileName = "Assets/Text/"+startGameEvent+".txt";
		GoToGameEvent(ge);
	
		Item.LoadItems("Assets/Items");

	}

	public static GameManager Instance
	{
		get{
			if (instance == null)
			{
				instance = new GameManager();
			}
			return instance;
		}
	}

	private void Update()
	{
		if (Input.GetButtonDown("Fire1"))
		{
			if (!currentGameEvent.EndOfGameEvent())
			{
				DisplayNextBox();
			}
			else
			{
				if (!buttonsDisplayed)
				{
					DisplayButtons();
				}
			}
		}
	}

	public void GoToGameEvent(GameEvent ge)
	{
		currentGameEvent = ge;
		ClearBoxes();
		ClearButtons();
		ge.Load();
		DisplayNextBox();

	}

	public bool DisplayNextBox()
	{
		if (currentGameEvent != null) {
			GameObject b = currentGameEvent.GetNextBox();
			if (b != null)
			{ 
				b.transform.SetParent(textPanel);
				return true;
			}
		}
		else
		{
			Debug.Log("Error: cannot display next box : currentGameEvent is null");
			
		}
		return false;
	}

	public void DisplayButtons()
	{
		for (int i = 0; i < currentGameEvent.nextGameEvents.Count; i++)
		{
			GameEvent nge = currentGameEvent.nextGameEvents[i];
			GameObject bo = Instantiate(buttonObject, buttonPanel);
			foreach (Operation op in currentGameEvent.nextOperations[i])
			{
				bo.GetComponent<Button>().onClick.AddListener(call: delegate { GameEvent.ApplyOperation(op); });
			}
			bo.GetComponent<Button>().onClick.AddListener(delegate { GoToGameEvent(nge); });
			bo.GetComponentInChildren<Text>().text = currentGameEvent.nextDescriptions[i];
		}
		buttonsDisplayed = true;
	}

	public void ClearBoxes()
	{
		for (int i = 0; i < textPanel.childCount; i++)
		{
			Destroy(textPanel.GetChild(i).gameObject);
		}
	}

	public void ClearButtons()
	{
		for (int i = 0; i < buttonPanel.childCount; i++)
		{
			Destroy(buttonPanel.GetChild(i).gameObject);
		}
		buttonsDisplayed = false;
	}

}
