using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public enum ConditionType { EQUALS, GREATER_THAN }
public enum OperationType { NONE, SET, ADD, CHANGE_MAP,CHANGE_CELL}

[System.Serializable]
public class Condition{
	public string key;
	public ConditionType conditionType;
	public string value;

	public bool IsVerified()
	{
		//key must be contained in values
		float value1;
		if (Values.ContainsKey(key))
		{
			Values.GetValueAsFloat(key, out value1);
		}
		else
		{
			throw new System.Exception("[GameEvent]" + key + " key is unknown");
		}
		//value must have a numeric value or be a key to a value
		float value2;
		if (!float.TryParse(value, out value2)) // is it a float ?
		{
			if (Values.ContainsKey(value)) //is it a key to a value ?
			{
				Values.GetValueAsFloat(value, out value2);
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
				throw new System.Exception("[Operation] Condition type not supported");
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
	public string value;

	public void Apply()
	{
		switch (operationType)
		{
			case OperationType.CHANGE_MAP:
				//GameManager.Instance.GoToMap(new Map(value));
				return;
			case OperationType.CHANGE_CELL:
				GameManager.Instance.GoToCell(value);
				return;
		}


		switch (operationType)
		{
			case OperationType.SET:
				Values.SetValueAsString(key, value.ToString());
				break;
			case OperationType.ADD:
				float v1, v2;
				if (!Values.GetValueAsFloat(key, out v1)) throw new System.Exception("[Operation] Cannot add : key " + key + " is not a float");
				if (!float.TryParse(value, out v2)) throw new System.Exception("[Operation] Cannot add : value " + value + " is not a float");
				Values.SetValueAsFloat(key, v1 + v2);
				break;
			default:
				throw new System.Exception("[Operation] Operation type not supported");
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
	[SerializeField]
	private string text;
	public List<Operation> operations;
	public List<Choice> choices;

	public string RawText{get{return text;} set{text = value;}}

	public string Text {
		get
		{
			string result = "";
			int textSize = text.Length;
			bool readingKey = false;
			string key = "";
			for(int i =0; i<textSize; i++)
			{
				if (!readingKey)
				{
					if(text[i] == '{')
					{
						readingKey = true;
					}
					else
					{
						result += text[i];
					}
				}
				else //readingKey
				{
					if (text[i] == '}')
					{
						string s;
						if (!Values.GetValueAsString(key, out s)) Debug.LogWarning("[GameEvent] Key " + key + " undefined.");
						result += s;
						readingKey = false;
						key = "";
					}
					else
					{
						key += text[i];
					}
				}
				
			}
			return result;
		}
	}

	public void ToGameObjects(out GameObject textBox, out List<GameObject> choiceBoxes)
	{
		textBox = GameObject.Instantiate(GameManager.Instance.textBox);
		Text text = textBox.transform.Find("Panel/Line").GetComponent<Text>();
		if (text == null) throw new System.Exception("[GameEvent] Cannot find Text component of TextBox prefab ");
		text.text = Text;

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