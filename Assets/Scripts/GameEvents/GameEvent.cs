using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public enum ConditionType {
	True,
	False,
	IsEqualTo,
	IsNotEqualTo,
	IsGreaterThan,
	IsLessThan,
	RandomChance //random chance between 0 (never) and 1 (always)
}
public enum OperationType {
	None, Set, Add, GoToMap, GoToCell, InitiateFight, PlayGameEvent, AddItem, RemoveItem, OpenMerchant, CloseMerchant, AddTeammate, Rest}

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
			case ConditionType.True: return (keyExists && Values.GetValueAsFloat(key,out float v) && v !=0);
			case ConditionType.False: return !keyExists;
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
			case OperationType.CloseMerchant:
				Inventory.Instance.CloseMerchantWindow();
				return;
			case OperationType.AddTeammate:
				GameManager.Instance.AddTeammate(other as Character);
				break;
			case OperationType.Rest:
				GameManager.Instance.PlayerTeam.Rest();
				break;
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

	private List<Operation> choiceOperations;
	private bool noChoice;
	private int currentParagraphId;

	public IEnumerator DisplayParagraph(int paragraphId = 0)
	{
		if (paragraphs.Count == 0) throw new System.Exception("Empty game event");
		currentParagraphId = paragraphId;

		Paragraph paragraph = paragraphs[paragraphId];

		choiceOperations = new List<Operation>();

		if (Condition.AreVerified(paragraph.conditions))
		{
			//instantiate text
			GameManager.Instance.CreateText(paragraph.Text);

			//apply paragraph operations
			foreach (var operation in paragraph.operations)
			{
				operation.Apply();
			}

			yield return new Prompt(DisplayChoices).Display();

			//apply choice operations
			foreach (var operation in choiceOperations)
			{
				operation.Apply();
			}

		}

		if (paragraphId >= paragraphs.Count - 1) // if last paragraph
		{
			if (noChoice && !(this is Location)) // if last displayed paragraph did not propose any choice and we are in a non location game event
			{
				yield return new Prompt(Prompt.PressOKToContinue).Display(); //prompt button before returning to map
			}
			yield break; 
		}
		else // else continue
		{
			yield return DisplayParagraph(++paragraphId);
		}
	}

	public void DisplayChoices(Prompt prompt)
	{
		noChoice = true;
		//instantiate choices
		foreach (Choice choice in paragraphs[currentParagraphId].choices)
		{
			if (!Condition.AreVerified(choice.conditions)) continue;

			GameManager.Instance.CreateButton(choice.text, delegate
			{
				choiceOperations.AddRange(choice.operations);
				prompt.Proceed();
			});
			noChoice = false;
		}
		if (noChoice) prompt.Proceed();
	}

}