using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	//Singleton instance
	private static GameManager instance = null;

	//Valeurs pour la lecture des gameEvent
	static public Dictionary<string, string> names = new Dictionary<string, string>();
	static public Dictionary<string, float> values = new Dictionary<string, float>();

	//Le joueur
	public Player player;

	//Les transform de l'UI
	public Transform textPanel;
	public Transform buttonPanel;
	public Transform infoPanel;
	private Text playerName;
	private Text playerHp;
	public Transform mapPanel;


	//L'événement actuel
	public string startGameEvent;
	public GameEvent currentGameEvent;
	public Fight currentFight;

	public string startMap;
	public Map currentMap;

	//Les prefabs
	public GameObject textBox;
	public GameObject dialogueBox;
	public GameObject buttonObject;
	public GameObject mapCellObject;
	
	//Etat de l'UI
	bool buttonsDisplayed = false;

	void Start () {
		//Singleton
		if(instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(this);
		}

		//Association des prefabs
		GameEvent.textBox = textBox;
		GameEvent.dialogueBox = dialogueBox;

		//Localisation des objets textes
		playerName = infoPanel.Find("Player Name").GetComponent<Text>();
		playerHp = infoPanel.Find("Player HP").GetComponent<Text>();



		//Valeurs initiales

	
		//Chargement des items
		Item.LoadItems("Assets/Items");

		//Création du joueur
		player = new Player();
		player.Name = "Mauri";
		player.Hp = 100;
		player.Weapon = (Weapon)Item.items["weapon"];

		UpdatePlayerInfo();

		Unit foe = new Unit();
		foe.Hp = 100;

		foe.Weapon = (Weapon)Item.items["weapon"];

		currentFight = new Fight(player, foe);
		//currentFight.Begin();

		//Premier GameEvent
		GameEvent ge = new GameEvent(startGameEvent);
		GoToGameEvent(ge);

		//Première Map
		Map map = new Map(startMap);
		GoToMap(map);

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
		if (currentGameEvent != null && Input.GetButtonDown("Fire1"))
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

	public void GoToMap(Map map)
	{
		currentMap = map;
		ClearMapPanel();
		map.Load();
		float w = mapCellObject.GetComponent<RectTransform>().rect.width;
		float h = mapCellObject.GetComponent<RectTransform>().rect.height;
		Debug.Log(map.Width);
		Debug.Log(map.Height);
		mapPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(w*map.Width, h*map.Height);

		for (int i = 0; i<map.Height; i++)
		{
			for(int j = 0; j<map.Width; j++)
			{
				if (map[i, j] != null)
				{
					GameObject go = Instantiate(mapCellObject, mapPanel);
					go.transform.localPosition = new Vector3(w * j, -i * h, 0);
				}
			}
		}
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
		ClearChilds(textPanel);
	}

	public void ClearButtons()
	{
		ClearChilds(buttonPanel);
		buttonsDisplayed = false;
	}

	public void ClearMapPanel()
	{
		ClearChilds(mapPanel);
	}

	static void ClearChilds(Transform t)
	{
		for (int i = 0; i < t.childCount; i++)
		{
			Destroy(t.GetChild(i).gameObject);
		}
	}

	public void UpdatePlayerInfo()
	{
		if(player != null)
		{
			playerName.text = player.Name;
			playerHp.text = player.Hp.ToString() + " / " + player.MaxHp.ToString();

		}
	}


	public static void ChangeValue(string key, float value)
	{
		if (values.ContainsKey(key))
		{
			values[key] = value;
		}
		else
		{
			values.Add(key, value);
		}

	}

	public static void CreateValue(string key)
	{
		if (!values.ContainsKey(key))
		{
			values.Add(key, 0);
		}
	}

	public static void ChangeName(string key, string value)
	{
		if (names.ContainsKey(key))
		{
			names[key] = value;
		}
		else
		{
			names.Add(key, value);
		}

	}

	public static void CreateName(string key)
	{
		if (!names.ContainsKey(key))
		{
			names.Add(key, "");
		}
	}

}
