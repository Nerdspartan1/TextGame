using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

//FORMER

enum ReadMode { TEXT, CONDITION, OPERATION, VALUE, NEXT_ADRESS, NEXT_DESC }
public enum ConditionType { EQUALS, GREATER_THAN }
public enum OperationType { NONE, ASSIGN, ADD, CHANGE_MAP,CHANGE_CELL}

//NEW
[System.Serializable]
public class Condition{
	public string key;
	public ConditionType conditionType;
	public string value;

	public bool IsVerified()
	{
		//key must be contained in values
		float value1;
		if (GameManager.values.ContainsKey(key))
		{
			value1 = GameManager.values[key];
		}
		else
		{
			throw new System.Exception("[GameEvent]" + key + " key is unknown");
		}
		//value must have a numeric value or a key to values
		float value2;
		if (!float.TryParse(value, out value2))
		{
			if (GameManager.values.ContainsKey(value))
			{
				value2 = GameManager.values[value];
			}
			else
			{
				throw new System.Exception("[GameEvent]" + value + " key is unknown");
			}
		}


		switch (conditionType)
		{
			case ConditionType.EQUALS:
				return (value1 == value2);
			case ConditionType.GREATER_THAN:
				return (value1 > value2);
			default:
				Debug.Log("Erreur : Condition inconnue");
				return false;
		}
	}

	public static bool AreVerified(List<Condition> conditions)
	{
		bool verified = true;
		foreach (Condition c in conditions)
		{
			verified &= c.IsVerified();
		}
		return verified;
	}
}

[System.Serializable]
public struct Operation
{
	public string key;
	public OperationType operationType;
	public float value;

	public void Apply()
	{
		switch (operationType)
		{
			case OperationType.CHANGE_MAP:
				GameManager.Instance.GoToMap(new Map(key));
				return;
			case OperationType.CHANGE_CELL:
				GameManager.Instance.GoToCell(key);
				return;
		}

		if (!GameManager.values.ContainsKey(key))
		{
			GameManager.CreateValue(key);
		}

		switch (operationType)
		{
			case OperationType.ASSIGN:
				GameManager.values[key] = value;
				break;
			case OperationType.ADD:
				GameManager.values[key] += value;
				break;
			default:
				throw new System.Exception("[Operation] Operation type unsupported");
		}


	}
}

[System.Serializable]
public class Choice
{
	public string text;
	public List<Operation> operations;

}

[System.Serializable]
public class Paragraph
{
	public List<Condition> conditions;
	[TextArea(5,15)]
	public string text;
	public List<Operation> operations;
	public List<Choice> choices;

	public void ToGameObjects(out GameObject textBox, out List<GameObject> choiceBoxes)
	{
		textBox = GameObject.Instantiate(GameManager.Instance.textBox);
		Text text = textBox.transform.Find("Panel/Line").GetComponent<Text>();
		if (text == null) throw new System.Exception("[GameEvent] Cannot find Text component of TextBox prefab ");
		text.text = this.text;

		choiceBoxes = new List<GameObject>();
		foreach(Choice choice in choices)
		{
			GameObject choiceBox = GameObject.Instantiate(GameManager.Instance.buttonObject);
			Button button = choiceBox.GetComponent<Button>();
			if(button == null) throw new System.Exception("[GameEvent] Cannot find Button component of Button prefab ");
			button.onClick.AddListener(delegate
			{
				foreach (Operation op in choice.operations)
				{
					op.Apply();
				}
				
			});
			Text buttonText = button.GetComponentInChildren<Text>();
			if (buttonText == null) throw new System.Exception("[GameEvent] Cannot find Text component of Button prefab ");
			buttonText.text = choice.text;

			choiceBoxes.Add(choiceBox);
		}

	} 

	public void ApplyOperations()
	{
		foreach(Operation op in operations)
		{
			op.Apply();
		}
	}
}


[CreateAssetMenu(fileName = "GameEvent",menuName = "ScriptableObjects/GameEvent")]
public class GameEvent : ScriptableObject
{
	public List<Paragraph> paragraphs  = new List<Paragraph>();
	int currentParagraphId;

	public void Init()
	{
		currentParagraphId = -1;
	}

	public Paragraph GetNextParagraph()
	{
		while (currentParagraphId < paragraphs.Count-1)
		{
			currentParagraphId++;
			Paragraph paragraph = paragraphs[currentParagraphId];
			

			if (Condition.AreVerified(paragraph.conditions))
			{
				return paragraph;
			}
			
		}
		return null;
	}

}