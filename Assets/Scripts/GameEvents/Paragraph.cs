using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Paragraph
{
	public List<Condition> conditions;
	[TextArea(10, 20)]
	[SerializeField]
	public string RawText;
	public List<Operation> operations;
	public List<Choice> choices;

	enum ReadMode
	{
		Text,
		Key,
		Condition
	}

	public string Text
	{
		get
		{
			string result = "";
			int textSize = RawText.Length;
			ReadMode readMode = ReadMode.Text;
			bool conditionMet = true;
			string key = "";
			for (int i = 0; i < textSize; i++)
			{
				switch(readMode)
				{
					case ReadMode.Text:
						if (RawText[i] == '<')
							readMode = ReadMode.Condition;
						else if (RawText[i] == '{')
							readMode = ReadMode.Key;
						else
						{
							result += RawText[i];
						}
						break;
					case ReadMode.Key:
						if (RawText[i] == '}')
						{
							string s;
							if (!Values.GetValueAsString(key, out s)) Debug.LogWarning($"[GameEvent] {key} key undefined.");
							result += s;
							readMode = ReadMode.Text;
							key = "";
						}
						else
						{
							key += RawText[i];
						}
						break;
					case ReadMode.Condition:
						if (RawText[i] == '>')
						{
							if (!key.StartsWith("/"))
							{
								bool not = key.StartsWith("!");

								if (Values.GetValueAsFloat(key.TrimStart('!'), out float value))
								{
									conditionMet = value != 0;
								}
								else // key not found
									conditionMet = false;

								if (not) conditionMet = !conditionMet;

								if (!conditionMet) // skip to the end of condition
								{
									string markup = $"</{key}>";
									int resumeIndex = RawText.IndexOf(markup, i);
									i = resumeIndex + markup.Length;
								}
							}
							readMode = ReadMode.Text;
							key = "";
						}
						else
						{
							key += RawText[i];
						}
						break;
				}

			}
			return result;
		}
	}

}
