using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CONST
{
	public const int defaultPropHeight = 16;
	public const int defaultInterlining = 2;
	public const int indentWidth = 16;
}

public class EditorUtils
{

	private static bool inHorizontalScope = false;

	public static bool InHorizontalScope
	{
		get
		{
			return inHorizontalScope;
		}
	}

	public static void BeginHorizontal()
	{
		if (inHorizontalScope) Debug.LogWarning("[GameEventEditor] Horizontal scope already begun.");
		EditorGUILayout.BeginHorizontal();
		inHorizontalScope = true;
	}

	public static void EndHorizontal()
	{
		if (!inHorizontalScope) Debug.LogWarning("[GameEventEditor] Horizontal scope not begun");
		EditorGUILayout.EndHorizontal();
		inHorizontalScope = false;
	}

	public static Rect PropertyList(Rect position, SerializedProperty property)
	{
		if (!property.isArray) throw new System.Exception("[PropertyList] Property argument must be an array");

		EditorGUI.indentLevel++;
		if (property.arraySize == 0)
		{
			Rect addButtonRect = new Rect(position.x + EditorGUI.indentLevel * CONST.indentWidth, position.y + CONST.defaultPropHeight, 100, CONST.defaultPropHeight);
			if (GUI.Button(addButtonRect, new GUIContent("Add property")))
			{
				property.InsertArrayElementAtIndex(0);
			}
		}
		else
		{
			for (int i = 0; i < property.arraySize; i++)
			{
				Rect propRect = new Rect(position.x, position.y + (i + 1) * (CONST.defaultPropHeight + CONST.defaultInterlining), position.width-40, CONST.defaultPropHeight);
				EditorGUI.PropertyField(propRect, property.GetArrayElementAtIndex(i));

				Rect addButtonRect = new Rect(propRect.x + propRect.width , propRect.y, 20, CONST.defaultPropHeight);
				if (GUI.Button(addButtonRect, new GUIContent("+")))
				{
					property.InsertArrayElementAtIndex(i);
					for (int j = property.arraySize - 1; j > i + 1; j--)
					{
						property.GetArrayElementAtIndex(j).isExpanded = property.GetArrayElementAtIndex(j - 1).isExpanded;
					}
					property.GetArrayElementAtIndex(i + 1).isExpanded = false;
				}
				Rect removeButtonRect = new Rect(propRect.x + propRect.width +20, propRect.y, 20, CONST.defaultPropHeight);
				if (GUI.Button(removeButtonRect, new GUIContent("-")))
				{
					bool isLastExpanded = property.GetArrayElementAtIndex(property.arraySize - 1).isExpanded;
					property.DeleteArrayElementAtIndex(i);
					for (int j = i; j < property.arraySize - 1; j++)
					{
						property.GetArrayElementAtIndex(j).isExpanded = property.GetArrayElementAtIndex(j + 1).isExpanded;
					}
					if (property.arraySize > 0)
						property.GetArrayElementAtIndex(property.arraySize - 1).isExpanded = isLastExpanded;
				}
			}
		}
		EditorGUI.indentLevel--;

		return new Rect(
			position.x, 
			position.y, 
			position.width, 
			property.arraySize > 0 ? 
				property.arraySize * (CONST.defaultPropHeight + CONST.defaultInterlining) :
				CONST.defaultPropHeight + CONST.defaultInterlining);
	}
	/*
	public static void PropertyListLayout(SerializedProperty property, bool foldout = false, bool shortProp = false)
	{
		PropertyListLayout(property, foldout, GUI.backgroundColor, shortProp);
	}

	public static void PropertyListLayout(SerializedProperty property, bool foldout, Color color, bool shortProp = false)
	{
		if (!property.isArray) throw new System.Exception("[PropertyList] Property argument must be an array");

		EditorGUI.indentLevel++;
		
		Color defaultColor = GUI.backgroundColor;

		if (property.arraySize == 0)
		{
			EditorUtils.BeginHorizontal();
			GUI.backgroundColor = color;

			GUILayout.Space(CONST.indentWidth * EditorGUI.indentLevel);
			if (GUILayout.Button(new GUIContent("Add property"), GUILayout.MaxWidth(100)))
			{
				property.InsertArrayElementAtIndex(0);
			}
			
			EditorUtils.EndHorizontal();
			GUI.backgroundColor = defaultColor;
		}
		else
		{

			for (int i = 0; i < property.arraySize; i++)
			{
				//display prop i

				if (shortProp)
					BeginHorizontal();

				SerializedProperty prop = property.GetArrayElementAtIndex(i);
				if (foldout)
				{
					prop.isExpanded = EditorGUILayout.Foldout(prop.isExpanded, GUIContent.none);
					if (prop.isExpanded)
					{
						EditorGUI.indentLevel++;
						EditorGUILayout.PropertyField(prop);
						EditorGUI.indentLevel--;

					}
				}
				else
				{
					EditorGUILayout.PropertyField(prop);
				}

				//buttons add and remove

				GUI.backgroundColor = color;
				if (!shortProp)
				{
					BeginHorizontal();
					GUILayout.Space(CONST.indentWidth * EditorGUI.indentLevel);
				}
				
				if (GUILayout.Button(new GUIContent("+"), GUILayout.MaxWidth(20)))
				{
					property.InsertArrayElementAtIndex(i);
					for (int j = property.arraySize-1; j > i+1 ; j--)
					{
						property.GetArrayElementAtIndex(j).isExpanded = property.GetArrayElementAtIndex(j - 1).isExpanded;
					}
					property.GetArrayElementAtIndex(i + 1).isExpanded = false;
				}
				if (GUILayout.Button(new GUIContent("-"), GUILayout.MaxWidth(20)))
				{
					bool isLastExpanded = property.GetArrayElementAtIndex(property.arraySize-1).isExpanded;
					property.DeleteArrayElementAtIndex(i);
					for (int j = i; j < property.arraySize-1; j++)
					{
						property.GetArrayElementAtIndex(j).isExpanded = property.GetArrayElementAtIndex(j + 1).isExpanded;
					}
					if(property.arraySize > 0)
						property.GetArrayElementAtIndex(property.arraySize-1).isExpanded = isLastExpanded;

				}

				EndHorizontal();
				GUI.backgroundColor = defaultColor;
			}
		}
		EditorGUI.indentLevel--;
		
	}*/
}

[CustomPropertyDrawer(typeof(Operation))]
public class OperationDrawer : PropertyDrawer
{
	static public float Width
	{
		get { return 320; }
	}
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), GUIContent.none);

		int indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;
		bool inHorizontalScope = EditorUtils.InHorizontalScope;

		Rect keyRect = new Rect(position.x, position.y, 100, CONST.defaultPropHeight);
		Rect operationRect = new Rect(position.x + 110, position.y, 100, CONST.defaultPropHeight);
		Rect valueRect = new Rect(position.x + 220, position.y, 100, CONST.defaultPropHeight);

		if (!inHorizontalScope)
		{
			EditorUtils.BeginHorizontal();
			GUILayout.Space(indent * CONST.indentWidth);
		}

		EditorGUI.PropertyField(keyRect, property.FindPropertyRelative("key"),GUIContent.none);
		EditorGUI.PropertyField(operationRect, property.FindPropertyRelative("operationType"), GUIContent.none);
		EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("value"), GUIContent.none);

		if (!inHorizontalScope)
			EditorUtils.EndHorizontal();
		EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty();
	}
}

[CustomPropertyDrawer(typeof(Condition))]
public class ConditionDrawer : PropertyDrawer
{
	static public float Width
	{
		get { return 320; }
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), GUIContent.none);

		int indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;
		bool inHorizontalScope = EditorUtils.InHorizontalScope;

		Rect keyRect = new Rect(position.x, position.y, 100, CONST.defaultPropHeight);
		Rect conditionRect = new Rect(position.x + 110, position.y, 100, CONST.defaultPropHeight);
		Rect valueRect = new Rect(position.x + 220, position.y, 100, CONST.defaultPropHeight);

		if (!inHorizontalScope)
		{
			EditorUtils.BeginHorizontal();
			GUILayout.Space(indent * CONST.indentWidth);
		}

		EditorGUI.PropertyField(keyRect, property.FindPropertyRelative("key"), GUIContent.none);
		EditorGUI.PropertyField(conditionRect, property.FindPropertyRelative("conditionType"), GUIContent.none);
		EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("value"), GUIContent.none);

		if (!inHorizontalScope)
			EditorUtils.EndHorizontal();
		EditorGUI.indentLevel = indent;


		EditorGUI.EndProperty();
	}
}

[CustomPropertyDrawer(typeof(Choice))]
public class ChoiceDrawer : PropertyDrawer
{
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		int size = property.FindPropertyRelative("operations").arraySize;
		return base.GetPropertyHeight(property, label) + ((size > 0) ? size * CONST.defaultPropHeight : 1 * CONST.defaultPropHeight);
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), GUIContent.none);

		int indentLevel = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, CONST.defaultPropHeight), property.FindPropertyRelative("text"), GUIContent.none);
		//EditorGUILayout.PropertyField(property.FindPropertyRelative("text"), GUIContent.none);

		EditorUtils.PropertyList(position, property.FindPropertyRelative("operations"));

		EditorGUI.indentLevel = indentLevel;

		EditorGUI.EndProperty();
	}
}

[CustomPropertyDrawer(typeof(Paragraph))]
public class ParagraphDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), GUIContent.none);

		//int indentLevel = EditorGUI.indentLevel;
		//EditorGUI.indentLevel = 0;

		SerializedProperty conditions = property.FindPropertyRelative("conditions");
		//conditions.isExpanded = EditorGUILayout.Foldout(conditions.isExpanded, new GUIContent("Conditions"));
		conditions.isExpanded = EditorGUI.Foldout(position, conditions.isExpanded, new GUIContent("Conditions"));
		if (conditions.isExpanded)
		{
			position.y += EditorUtils.PropertyList(position, conditions).height;
			position.y += 20;
		}

		
		EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, 100), property.FindPropertyRelative("text"));
		position.y += 100;
		//EditorGUILayout.PropertyField(property.FindPropertyRelative("text"));

		SerializedProperty operations = property.FindPropertyRelative("operations");
		operations.isExpanded = EditorGUI.Foldout(position, operations.isExpanded, new GUIContent("Operations"));
		if (operations.isExpanded)
		{
			position.y += EditorUtils.PropertyList(position, operations).height;
			position.y += 20;
		}
		SerializedProperty choices = property.FindPropertyRelative("choices");
		choices.isExpanded = EditorGUI.Foldout(position, choices.isExpanded, new GUIContent("Choices"));
		if (choices.isExpanded)
		{
			position.y += EditorUtils.PropertyList(position, choices).height;
			position.y += 20;
		}

		//EditorGUI.indentLevel = indentLevel;
		

		EditorGUI.EndProperty();
	}
}

[CustomEditor(typeof(GameEvent))]
public class GameEventEditor : Editor
{
	/*SerializedProperty paragraphs;

	private void OnEnable()
	{
		paragraphs = serializedObject.FindProperty("paragraphs");
	}

	public override void OnInspectorGUI()
	{
		EditorUtils.PropertyListLayout(paragraphs, true, 1f * Color.red + 0.6f * Color.green);

		if (GUILayout.Button("Save changes"))
		{
			serializedObject.ApplyModifiedProperties();
		}

	}
	*/
}