using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	private static GameManager instance = null;

	static public Dictionary<string, string> names = new Dictionary<string, string>();
	static public Dictionary<string, float> values = new Dictionary<string, float>();

	public Text mainText;
	public Transform buttonPanel;
	public GameObject buttonObject;
	public Fight currentFight;

	void Start () {
		if(instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(this);
		}

		names.Add("name", "Maurice");
		values.Add("isHappy", 1);
		values.Add("isBored", 1);
		
		GameEvent ge = new GameEvent();
		ge.fileName = "Assets/Text/start.txt";
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

	public void GoToGameEvent(GameEvent ge)
	{
		ClearButtons();
		ge.Load();
		mainText.text = ge.text;
		for (int i=0; i< ge.nextGameEvents.Count; i++)
		{
			GameEvent nge = ge.nextGameEvents[i];
			GameObject bo = Instantiate(buttonObject, buttonPanel);
			foreach(Operation op in ge.nextOperations[i])
			{
				bo.GetComponent<Button>().onClick.AddListener(call: delegate { GameEvent.ApplyOperation(op); });
			}
			bo.GetComponent<Button>().onClick.AddListener(delegate { GoToGameEvent(nge); });
			bo.GetComponentInChildren<Text>().text = ge.nextDescriptions[i];
		}
	}

	public void ClearButtons()
	{
		for (int i = 0; i < buttonPanel.childCount; i++)
		{
			Destroy(buttonPanel.GetChild(i).gameObject);
		}
	}

}
