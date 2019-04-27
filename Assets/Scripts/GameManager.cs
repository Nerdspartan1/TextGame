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
	
	[Header("Player")]
	public Player player;

	[HideInInspector]
	public FightManager fightManager;

	//UI
	[Header("UI References")]
	public Transform textPanel;
	public Transform buttonPanel;
	public Transform infoPanel;
	public Transform mapHidingPanel;
	public Text playerNameInfoText;
	public Text playerHpInfoText;
	public Text playerLevelInfoText;
	public Text playerXpInfoText;
	public Transform mapPanel;
	Dictionary<Vector2,Button> mapCells = new Dictionary<Vector2,Button>();
	private float cellWidth, cellHeight;
	private bool buttonsDisplayed = false;

	//Les prefabs
	[Header("Prefabs")]
	public GameObject textBox;
	public GameObject dialogueBox;
	public GameObject buttonObject;
	public GameObject mapCellObject;
	public GameObject playerCursor;


	//L'événement actuel
	[Header("Initial values")]
	public GameEvent startGameEvent;
	private GameEvent currentGameEvent;
	public string startMap;
	private Map currentMap;
	public string startLocation;
	[HideInInspector]
	public GameEvent currentLocation;
	private Vector2 currentCellPos;

	//Debug
	[Header("##DEBUG##")]
	public Enemy foe;
	

	void Start () {
		//Singleton
		if(instance == null)
			instance = this;
		else
			Destroy(this);

		//Référencement des autres managers
		fightManager = GetComponent<FightManager>();

		player.Init();
		GoToGameEvent(startGameEvent);

		//Première Map
		//Map map = new Map(startMap);
		//GoToMap(map);

		//GoToCell("labr1");

		//GoToGameEvent(new GameEvent("start"));
		//fightManager.BeginFight(foe);

		//Actualise les infos du joueur sur les textes à l'écran
		UpdatePlayerInfo();

	}

	public static GameManager Instance
	{
		get{
			return instance;
		}
	}

	private void Update()
	{
		UpdatePlayerInfo();
		if (currentGameEvent != null && Input.GetButtonDown("Fire1"))
		{
			if (!buttonsDisplayed)
			{
				DisplayNextBox();
			}
		}
	}

	public void GoToGameEvent(GameEvent ge)
	{
		if (ge == null) //Si le gameEvent est null, alors on retourne en mode exploration
		{
			return;
			
		}
		currentGameEvent = ge;
		currentGameEvent.Init();

		ClearBoxes();
		ClearButtons();

		DisplayNextBox();
	}

	public bool DisplayNextBox()
	{
		
		if (currentGameEvent != null)
		{
			Paragraph p = currentGameEvent.GetNextParagraph();
			if (p != null)
			{
				GameObject textBox;
				List<GameObject> choiceBoxes;
				p.ToGameObjects(out textBox, out choiceBoxes);
				textBox.transform.SetParent(textPanel);

				foreach (GameObject choiceBox in choiceBoxes)
				{
					choiceBox.transform.SetParent(buttonPanel);
					choiceBox.GetComponent<Button>().onClick.AddListener(delegate
					{
						ClearButtons();
						DisplayNextBox();
					});
					buttonsDisplayed = true;
				}
				return true;
			}
		}
		else
		{
			throw new System.Exception("No current GameEvent. Cannot display next paragraph");

		}
		return false;
	}

	public void GoToMap(Map map)
	{
		currentMap = map;
		ClearMapPanel();
		//map.Load();
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
					//go.GetComponent<Button>().onClick.AddListener(delegate { GoToCell(ExtractFileName(map[_i, _j].path)); });
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
		/*
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
		*/
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
		playerCursor.transform.SetParent(null);
		ClearChilds(mapPanel);
		playerCursor.transform.SetParent(mapPanel);
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
			playerNameInfoText.text = player.Name;
			playerHpInfoText.text = player.Hp.ToString() + " / " + player.MaxHp.ToString();
			playerLevelInfoText.text = "Lvl. " + player.Level.ToString();
			playerXpInfoText.text = "XP : " + player.Xp.ToString();

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
		ChangeValue(key, 0);
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
		ChangeName(key, "");
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
