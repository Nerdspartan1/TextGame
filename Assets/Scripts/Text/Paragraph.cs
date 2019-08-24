using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Paragraph
{
	[TextArea(5, 15)]
	[SerializeField]
	private string text;
	public List<Operation> operations;

	public string RawText { get { return text; } set { text = value; } }

	public string Text
	{
		get
		{
			string result = "";
			int textSize = text.Length;
			bool readingKey = false;
			string key = "";
			for (int i = 0; i < textSize; i++)
			{
				if (!readingKey)
				{
					if (text[i] == '{')
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
