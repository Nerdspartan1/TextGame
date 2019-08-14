using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public enum ConditionType { Exists, DoesNotExist, IsEqualTo, IsNotEqualTo, IsGreaterThan }
public enum OperationType { None, Set, Add, GoToMap, GoToCell, InitiateFight, PlayGameEvent, AddItem, RemoveItem}

[System.Serializable]
public struct Condition{
	public string key;
	public ConditionType conditionType;
	public string value;

	public bool IsVerified()
	{
		//trim the strings
		key = key.Trim();
		value = value.Trim();

		bool keyExists = Values.ContainsKey(key);

		if		(conditionType == ConditionType.Exists)		  return keyExists;
		else if (conditionType == ConditionType.DoesNotExist) return !keyExists;

		//key must be contained in Values
		if (!keyExists)
		{
			Debug.LogError("[GameEvent]" + key + " key is unknown");
			return false;
		}

		float value1;
		if(!Values.GetValueAsFloat(key, out value1))
		{
			Debug.LogError("[GameEvent]" + key + " key is not float");
			return false;
		}

		//value must have a numeric value or be a key to a value
		float value2;
		if (!Values.GetValueAsFloat(value, out value2)) // is it a float ?
		{
			if (!Values.ContainsKey(value)) //is it a key to a value ?
			{
				//if both checks failed, error
				Debug.LogError("[GameEvent]" + value + " key is unknown");
				return false;
			}
		}


		switch (conditionType)
		{
			case ConditionType.IsEqualTo:
				return (value1 == value2);
			case ConditionType.IsNotEqualTo:
				return (value1 != value2);
			case ConditionType.IsGreaterThan:
				return (value1 > value2);
			default:
				Debug.LogError("[Operation] Condition type not supported");
				return false;
		}
	}

	public static bool AreVerified(IEnumerable<Condition> conditions)
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
	public Object reference;
	public Vector2Int position;

	public void Apply()
	{
		switch (operationType)
		{
			case OperationType.GoToMap:
				GameManager.Instance.ExitGameEvent();
				GameManager.Instance.GoToMap((Map)reference);
				return;
			case OperationType.GoToCell:
				GameManager.Instance.ExitGameEvent();
				GameManager.Instance.GoToLocation(position);
				return;
			case OperationType.InitiateFight:
				GameManager.Instance.ExitGameEvent();
				if(reference is Team)
					FightManager.Instance.BeginFight((Team)reference);
				else
					FightManager.Instance.BeginFight((Enemy)reference);
				return;
			case OperationType.PlayGameEvent:
				GameManager.Instance.ExitGameEvent();
				GameManager.Instance.PlayGameEvent((GameEvent)reference);
				return;
			case OperationType.Set:
				Values.SetValueAsString(key, value.ToString());
				return;
			case OperationType.Add:
				float v1, v2;
				if (!Values.GetValueAsFloat(key, out v1)) throw new System.Exception($"[Operation] Cannot add : key {key} is not a float");
				if (!float.TryParse(value, out v2)) throw new System.Exception($"[Operation] Cannot add : value {value} is not a float");
				Values.SetValueAsFloat(key, v1 + v2);
				return;
			case OperationType.AddItem:
				Inventory.Instance.Add((Item)reference);
				return;
			case OperationType.RemoveItem:
				Inventory.Instance.Remove((Item)reference);
				return;
			default:
				throw new System.Exception("[Operation] Operation type not supported");
		}


	}

	public static void ApplyAll(IEnumerable<Operation> operations)
	{
		foreach(Operation operation in operations)
		{
			operation.Apply();
		}
	}

}

[System.Serializable]
public struct Choice
{
	public List<Condition> conditions;
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
						if (!Values.GetValueAsString(key, out s)) Debug.LogWarning($"[GameEvent] {key} key undefined.");
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

	public void ApplyOperations()
	{
		Operation.ApplyAll(operations);
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