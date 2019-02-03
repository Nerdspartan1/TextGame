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

	//UI
	public Transform textPanel;
	public Transform buttonPanel;
	public Transform infoPanel;
	public Transform mapHidingPanel;
	private Text playerName;
	private Text playerHp;
	public Transform mapPanel;
	Dictionary<Vector2,Button> mapCells = new Dictionary<Vector2,Button>();
	private float cellWidth, cellHeight;


	//L'événement actuel
	public string startGameEvent;
	public GameEvent currentGameEvent;

	public Fight currentFight;

	public string startMap;
	public Map currentMap;
	public string startLocation;
	public GameEvent currentLocation;
	Vector2 currentCellPos;

	//Les prefabs
	public GameObject textBox;
	public GameObject dialogueBox;
	public GameObject buttonObject;
	public GameObject mapCellObject;
	public GameObject playerCursor;
	
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
		//GameEvent ge = new GameEvent(startGameEvent);
		//GoToGameEvent(ge);

		//Première Map
		Map map = new Map(startMap);
		GoToMap(map);

		GoToCell("labr1");

		GoToGameEvent(new GameEvent("start"));

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
		if (ge == null) //Si le gameEvent est null, alors on retourne en mode exploration
		{
			ge = currentLocation;
		}
		currentGameEvent = ge;
		ClearBoxes();
		ClearButtons();
		ge.Load();
		mapHidingPanel.gameObject.SetActive(!ge.IsMapLocation);
		DisplayNextBox();
	}

	public void GoToMap(Map map)
	{
		currentMap = map;
		ClearMapPanel();
		map.Load();
		cellWidth = mapCellObject.GetComponent<RectTransform>().rect.width;
		cellHeight = mapCellObject.GetComponent<RectTransform>().rect.height;

		mapPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(cellWidth*map.Width, cellHeight*map.Height);
		mapCells.Clear();

		for (int i = 0; i<map.Height; i++)
		{
			for(int j = 0; j<map.Width; j++)
			{
				if (map[i, j] != null)
				{
					GameObject go = Instantiate(mapCellObject, mapPanel);
					go.transform.localPosition = new Vector3(cellWidth * j, -i * cellHeight, 0);
					int _i = i;
					int _j = j;
					go.GetComponent<Button>().onClick.AddListener(delegate { GoToCell(ExtractFileName(map[_i, _j].path)); });
					mapCells.Add(new Vector2(_i, _j), go.GetComponent<Button>());
				}
			}
		}
	}

	/// <summary>
	/// Goes the cell named cellName within the current map
	/// </summary>
	/// <param name="cellName"></param>
	public void GoToCell(string cellName)
	{
		if(currentMap != null)
		{
			GameEvent cell = currentMap.Find(cellName);
			if(cell != null)
			{
				Vector2 position = currentMap.GetPosition(cellName);
				playerCursor.transform.localPosition = new Vector3(position.y * cellWidth, -position.x * cellHeight);

				GoToGameEvent(cell);
				currentLocation = cell;
				while (!currentGameEvent.EndOfGameEvent())
				{
					DisplayNextBox();
				}
				foreach(KeyValuePair<Vector2,Button> pair in mapCells)
				{
					if((pair.Key.x == position.x+1 && pair.Key.y == position.y)
						|| (pair.Key.x == position.x-1 && pair.Key.y == position.y)
						|| (pair.Key.y == position.y+1 && pair.Key.x == position.x)
						|| (pair.Key.y == position.y-1 && pair.Key.x == position.x))
					{
						pair.Value.interactable = true;
					}
					else
					{
						pair.Value.interactable = false;
					}
				}

			}
			else
			{
				Debug.Log("Cellule introuvable : "+cellName);
			}
		}
		else
		{
			Debug.Log("Impossible de chercher une cellule : map non référencée.");
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
		playerCursor.transform.parent = null;
		ClearChilds(mapPanel);
		playerCursor.transform.parent = mapPanel;
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

	public static string ExtractFileName(string filePath)
	{
		string result = filePath;
		for (int i = filePath.Length - 1; i >= 0; i--)
		{
			if (filePath[i] == '.')
			{
				result = result.Remove(i);
			}
			if (filePath[i] == '/' || filePath[i] == '\\')
			{
				result = result.Remove(0, i + 1);
				break;
			}

		}
		return result;
	}

}
