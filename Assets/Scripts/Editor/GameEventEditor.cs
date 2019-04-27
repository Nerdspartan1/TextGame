using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CONST
{
	public const int defaultPropHeight = 16;
	public const int indentWidth = 16;
}

[CustomPropertyDrawer(typeof(Operation))]
public class OperationDrawer : PropertyDrawer
{
	static public float Width
	{
		get { return 280; }
	}
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), GUIContent.none);

		var keyRect =	new Rect(position.x,			position.y, 120, position.height);
		var typeRect =	new Rect(position.x + 125,		position.y, 80, position.height);
		var valueRect = new Rect(position.x + 210,		position.y, 50, position.height);

		int indentLevel = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		EditorGUI.PropertyField(keyRect, property.FindPropertyRelative("key"),GUIContent.none);
		EditorGUI.PropertyField(typeRect, property.FindPropertyRelative("operationType"), GUIContent.none);
		EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("value"), GUIContent.none);

		EditorGUI.indentLevel = indentLevel;

		EditorGUI.EndProperty();
	}
}

[CustomPropertyDrawer(typeof(Choice))]
public class ChoiceDrawer : PropertyDrawer
{
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		int size = property.FindPropertyRelative("operations").arraySize;
		return base.GetPropertyHeight(property,label) + ((size > 0) ? size*CONST.defaultPropHeight : 1*CONST.defaultPropHeight);
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), GUIContent.none);

		int indentLevel = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		EditorGUI.PropertyField(new Rect(position.x,position.y,position.width,CONST.defaultPropHeight), property.FindPropertyRelative("text"), GUIContent.none);

		SerializedProperty operations = property.FindPropertyRelative("operations");
		EditorGUI.indentLevel++;
		if (operations.arraySize == 0)
		{
			Rect addButtonRect = new Rect(position.x+EditorGUI.indentLevel*CONST.indentWidth, position.y+CONST.defaultPropHeight, 100, CONST.defaultPropHeight);
			if (GUI.Button(addButtonRect, new GUIContent("Add operations")))
			{
				operations.InsertArrayElementAtIndex(0);
			}
		}
		else
		{
			for (int i = 0; i < operations.arraySize; i++)
			{
				Rect operationRect = new Rect(position.x, position.y + (i + 1) * CONST.defaultPropHeight, OperationDrawer.Width, CONST.defaultPropHeight);
				EditorGUI.PropertyField(operationRect, operations.GetArrayElementAtIndex(i));

				Rect addButtonRect = new Rect(operationRect.x + operationRect.width, operationRect.y, 20, CONST.defaultPropHeight);
				if (GUI.Button(addButtonRect, new GUIContent("+")))
				{
					operations.InsertArrayElementAtIndex(i);
				}
				Rect removeButtonRect = new Rect(operationRect.x + operationRect.width + addButtonRect.width, operationRect.y, 20, CONST.defaultPropHeight);
				if (GUI.Button(removeButtonRect, new GUIContent("-")))
				{
					operations.DeleteArrayElementAtIndex(i);
				}
			}
		}

		EditorGUI.indentLevel = EditorGUI.indentLevel;

		EditorGUI.EndProperty();
	}
}

[CustomPropertyDrawer(typeof(Condition))]
public class ConditionDrawer : PropertyDrawer
{
	static public float Width
	{
		get { return 280; }
	}
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), GUIContent.none);

		var keyRect = new Rect(position.x, position.y, 120, position.height);
		var typeRect = new Rect(position.x + 125, position.y, 80, position.height);
		var valueRect = new Rect(position.x + 210, position.y, 50, position.height);

		int indentLevel = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		EditorGUI.PropertyField(keyRect, property.FindPropertyRelative("key"), GUIContent.none);
		EditorGUI.PropertyField(typeRect, property.FindPropertyRelative("conditionType"), GUIContent.none);
		EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("value"), GUIContent.none);

		EditorGUI.indentLevel = indentLevel;

		EditorGUI.EndProperty();
	}
}