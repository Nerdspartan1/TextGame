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
			Debug.LogError($"'{key}' is unknown");
			return false;
		}

		//key must be a key to a float
		float v1;
		if(!Values.GetValueAsFloat(key, out v1))
		{
			Debug.LogError($"'{key}' is not a float");
			return false;
		}

		//value must be a float or be a key to a float
		float v2;
		if (!float.TryParse(value, out v2)) // is it a float ?
		{
			if (!Values.GetValueAsFloat(value, out v2)) //is it a key to a float ?
			{
				//if both checks failed, error
				Debug.LogError($"'{value}' is unknown");
				return false;
			}
		}


		switch (conditionType)
		{
			case ConditionType.IsEqualTo:
				return (v1 == v2);
			case ConditionType.IsNotEqualTo:
				return (v1 != v2);
			case ConditionType.IsGreaterThan:
				return (v1 > v2);
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
				GameManager.Instance.GoToMap((Map)reference);
				return;
			case OperationType.GoToCell:
				GameManager.Instance.GoToLocation(position);
				return;
			case OperationType.InitiateFight:
				if(reference is Team)
					FightManager.Instance.BeginFight((Team)reference);
				else
					FightManager.Instance.BeginFight((Enemy)reference);
				return;
			case OperationType.PlayGameEvent:
				GameManager.Instance.PlayGameEvent((GameEvent)reference);
				return;
			case OperationType.Set:
				Values.SetValueAsString(key, value.ToString());
				return;
			case OperationType.Add:
				float v1, v2;
				Values.GetValueAsFloat(key, out v1); //0 if key does not exist
				if (!float.TryParse(value, out v2)) throw new System.Exception($"[Operation] Cannot add : '{value}' is not a float");
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

[CreateAssetMenu(fileName = "GameEvent",menuName = "ScriptableObjects/GameEvent")]
public class GameEvent : ScriptableObject
{
	public List<Paragraph> paragraphs  = new List<Paragraph>();
	int currentParagraphId = -1;

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

	public bool HasNextParagraph { get => currentParagraphId < paragraphs.Count - 1; }

}