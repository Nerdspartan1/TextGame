using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Paragraph
{
	[TextArea(5, 15)]
	[SerializeField]
	public string RawText;
	public List<Operation> operations;

	public string Text
	{
		get
		{
			string result = "";
			int textSize = RawText.Length;
			bool readingKey = false;
			string key = "";
			for (int i = 0; i < textSize; i++)
			{
				if (!readingKey)
				{
					if (RawText[i] == '{')
					{
						readingKey = true;
					}
					else
					{
						result += RawText[i];
					}
				}
				else //readingKey
				{
					if (RawText[i] == '}')
					{
						string s;
						if (!Values.GetValueAsString(key, out s)) Debug.LogWarning($"[GameEvent] {key} key undefined.");
						result += s;
						readingKey = false;
						key = "";
					}
					else
					{
						key += RawText[i];
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
