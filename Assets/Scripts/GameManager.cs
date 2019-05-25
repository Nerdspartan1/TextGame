using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class Values
{
	static Dictionary<string, string> values = new Dictionary<string, string>();

	static public bool ContainsKey(string key)
	{
		return values.ContainsKey(key);
	}

	static public bool GetValueAsFloat(string key, out float value)
	{
		string s;
		value = 0;
		if (!values.TryGetValue(key, out s)) return false;
		if (!float.TryParse(s, out value)) return false;
		return true;
	}

	static public void SetValueAsFloat(string key, float value)
	{
		if(values.ContainsKey(key))
		{
			values[key] = value.ToString();
		}
		else
		{
			values.Add(key, value.ToString());
		}
	}

	static public bool GetValueAsString(string key, out string value)
	{
		return values.TryGetValue(key, out value);
	}

	static public void SetValueAsString(string key, string value)
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

}

public class GameManager : MonoBehaviour {
	//Singleton instance
	private static GameManager instance = null;
	
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
	Dictionary<Vector2Int,Button> MapCells = new Dictionary<Vector2Int,Button>();
	private float cellWidth, cellHeight;
	private bool buttonsDisplayed = false;

	//Les prefabs
	[Header("Prefabs")]
	public GameObject TextBoxPrefab;
	public GameObject DialogueBoxPrefab;
	public GameObject ButtonPrefab;
	public GameObject LocationPrefab;
	public GameObject MapCursorPrefab;


	//L'événement actuel
	[Header("Initial values")]
	public GameEvent StartingGameEvent;
	private GameEvent currentGameEvent;
	public Map StartingMap;
	private Map currentMap;
	public Vector2Int StartingLocation;
	private Vector2Int currentLocation;

	//Debug
	[Header("Debug")]
	public Enemy foe;
	

	void Start () {
		//Singleton
		if(instance == null)
			instance = this;
		else
			Destroy(this);

		ClearText();
		ClearButtons();
		ClearMapPanel();

		//Référencement des autres managers
		fightManager = GetComponent<FightManager>();

		player.Init();
		if(StartingGameEvent != null)
			PlayGameEvent(StartingGameEvent);

		//starting map
		GoToMap(StartingMap);

		GoToLocation(StartingLocation.x,StartingLocation.y);

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
				DisplayNextParagraphInGameEvent();
			}
		}
	}

	public void PlayGameEvent(GameEvent ge)
	{
		//Si le gameEvent est null, alors on retourne en mode exploration
		if (ge == null) return;

		currentGameEvent = ge;
		currentGameEvent.Init();

		ClearText();
		ClearButtons();

		DisplayNextParagraphInGameEvent();
	}

	public void DisplayParagraph(Paragraph paragraph, UnityAction onNext = null)
	{
		if (paragraph == null) return;

		//instantiate text
		GameObject textBox;
		textBox = Instantiate(TextBoxPrefab, textPanel);
		Text text = textBox.transform.Find("Panel/Line").GetComponent<Text>();
		if (text == null) throw new System.Exception("[GameManager] Cannot find Text component of TextBox prefab ");
		text.text = paragraph.Text;

		//instantiate choices
		foreach (Choice choice in paragraph.choices)
		{
			if (!Condition.AreVerified(choice.conditions)) continue;

			GameObject choiceBox = Instantiate(ButtonPrefab,buttonPanel);
			Button button = choiceBox.GetComponent<Button>();
			if (button == null) throw new System.Exception("[GameManager] Cannot find Button component of Button prefab ");
			Text buttonText = button.GetComponentInChildren<Text>();
			if (buttonText == null) throw new System.Exception("[GameManager] Cannot find Text component of Button prefab ");

			buttonText.text = choice.text;
	
			button.onClick.AddListener(delegate
			{
				foreach (Operation op in choice.operations)
				{
					op.Apply();
				}
				ClearButtons();
			});
			if (onNext != null) button.onClick.AddListener(onNext);
			
			buttonsDisplayed = true;
		}

		//apply operations
		paragraph.ApplyOperations();
	}

	public bool DisplayNextParagraphInGameEvent()
	{
		if (currentGameEvent == null) throw new System.Exception("No current GameEvent. Cannot display next paragraph");

		Paragraph p = currentGameEvent.GetNextParagraph();
		if (p == null) return false;

		DisplayParagraph(p, delegate { DisplayNextParagraphInGameEvent(); });

		return true;
		
	}

	public void GoToMap(Map map)
	{
		currentMap = map;
		ClearMapPanel();

		cellWidth = LocationPrefab.GetComponent<RectTransform>().rect.width;
		cellHeight = LocationPrefab.GetComponent<RectTransform>().rect.height;

		mapPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(cellWidth*map.Width, cellHeight*map.Height);
		MapCells.Clear();
		
		for (int v = 0; v<map.Height; v++)
		{
			for(int u = 0; u<map.Width; u++)
			{
				if (map[u, v] != null)
				{
					GameObject go = Instantiate(LocationPrefab, mapPanel);
					go.transform.localPosition = new Vector3(cellWidth * u, -v * cellHeight, 0);
					int _u = u;
					int _v = v;
					go.GetComponent<Button>().onClick.AddListener(delegate { GoToLocation(_u,_v); });
					MapCells.Add(new Vector2Int(_u, _v), go.GetComponent<Button>());
				}
			}
		}
	}

	public void GoToLocation(int u, int v)
	{
		if(currentMap == null)
		{
			Debug.LogError($"[GameManager] Cannot move: no map provided");
			return;
		}
		
		Location location = currentMap[u, v];
		if(location == null)
		{
			Debug.LogError($"[GameManager] No location found at ({u},{v})");
			return;
		}

		MapCursorPrefab.transform.localPosition = new Vector2(u * cellWidth, -v* cellHeight);

		//Display location paragraph here
		DisplayParagraph(location.description);

		//set the possible destinations
		foreach(KeyValuePair<Vector2Int,Button> pair in MapCells)
		{
			pair.Value.interactable = false;
		}
		currentLocation = new Vector2Int(u, v);
		Button b;
		if (MapCells.TryGetValue(currentLocation + Vector2Int.up, out b)) b.interactable = true;
		if (MapCells.TryGetValue(currentLocation + Vector2Int.right, out b)) b.interactable = true;
		if (MapCells.TryGetValue(currentLocation + Vector2Int.down, out b)) b.interactable = true;
		if (MapCells.TryGetValue(currentLocation + Vector2Int.left, out b)) b.interactable = true;

	}

	public void ClearText()
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
		MapCursorPrefab.transform.SetParent(null);
		ClearChilds(mapPanel);
		MapCursorPrefab.transform.SetParent(mapPanel);
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
		if (player == null) return;

		playerNameInfoText.text = player.Name;
		playerHpInfoText.text = player.Hp.ToString() + " / " + player.MaxHp.ToString();
		playerLevelInfoText.text = "Lvl. " + player.Level.ToString();
		playerXpInfoText.text = "XP : " + player.Xp.ToString();

	}

}
