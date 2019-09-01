using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public enum ConditionType {
	Exists,
	DoesNotExist,
	IsEqualTo,
	IsNotEqualTo,
	IsGreaterThan,
	IsLessThan,
	RandomChance //random chance between 0 (never) and 1 (always)
}
public enum OperationType { None, Set, Add, GoToMap, GoToCell, InitiateFight, PlayGameEvent, AddItem, RemoveItem, OpenMerchant}

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

		switch (conditionType)
		{
			case ConditionType.Exists: return keyExists;
			case ConditionType.DoesNotExist: return !keyExists;
		}

		float v1 = 0;
		if (conditionType != ConditionType.RandomChance)
		{
			//key must be contained in Values
			if (!keyExists)
			{
				Debug.LogError($"'{key}' is unknown");
				return false;
			}

			//key must be a key to a float
			if (!Values.GetValueAsFloat(key, out v1))
			{
				Debug.LogError($"'{key}' is not a float");
				return false;
			}
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
			case ConditionType.IsLessThan:
				return (v1 < v2);
			case ConditionType.RandomChance:
				return (Random.value < v2);
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
	public GameEvent gameEvent;
	public Vector2Int position;
	public Item item;
	public Merchant merchant;
	public Object other;

	public void Apply()
	{
		switch (operationType)
		{
			case OperationType.GoToMap:
				GameManager.Instance.GoToMap((Map)other);
				return;
			case OperationType.GoToCell:
				GameManager.Instance.GoToLocation(position);
				return;
			case OperationType.InitiateFight:
				if (other is Enemy enemy) FightManager.Instance.BeginFight(enemy, gameEvent);
				else if (other is Team team) FightManager.Instance.BeginFight(team, gameEvent);
				else throw new System.Exception("Bad type");
				return;
			case OperationType.OpenMerchant:
				Inventory.Instance.OpenMerchantWindow(merchant);
				return;
			case OperationType.PlayGameEvent:
				GameManager.Instance.PlayGameEvent(gameEvent);
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
				Inventory.Instance.Add(item);
				return;
			case OperationType.RemoveItem:
				Inventory.Instance.Remove(item);
				return;
			default:
				throw new System.Exception("[Operation] Operation type not supported");
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

	private List<Operation> operationsToApply;
	private int currentParagraphId;

	public IEnumerator DisplayParagraph(int paragraphId = 0)
	{
		if (paragraphId >= paragraphs.Count) yield break; // break if current paragraph doesn't exist

		currentParagraphId = paragraphId;

		Paragraph paragraph = paragraphs[paragraphId];

		operationsToApply = new List<Operation>();

		if (Condition.AreVerified(paragraph.conditions))
		{
			//instantiate text
			GameManager.Instance.CreateText(paragraph.Text);

			yield return new Prompt(DisplayChoices).Display();

			operationsToApply.AddRange(paragraph.operations);
				
		}

		foreach(var operation in operationsToApply)
		{
			operation.Apply();
			switch (operation.operationType)
			{
				case OperationType.InitiateFight:
				case OperationType.PlayGameEvent:
					yield break;
			}
		}
		
		yield return DisplayParagraph(++paragraphId);
		
	}

	public void DisplayChoices(Prompt prompt)
	{
		bool noChoice = true;
		//instantiate choices
		foreach (Choice choice in paragraphs[currentParagraphId].choices)
		{
			if (!Condition.AreVerified(choice.conditions)) continue;

			GameManager.Instance.CreateButton(choice.text, delegate
			{
				operationsToApply.AddRange(choice.operations);
				prompt.Proceed();
			});
			noChoice = false;
		}
		if(noChoice)
		{
			//if not location and last paragraph, prompt button before returning to map
			if (!(this is Location) && currentParagraphId == paragraphs.Count - 1)
				prompt.Next = new Prompt(Prompt.PressOKToContinue); 
			prompt.Proceed();
		}
	}

}