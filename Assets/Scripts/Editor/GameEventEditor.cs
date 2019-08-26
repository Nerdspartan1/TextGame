using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class K
{
	public const int defaultPropHeight = 16;
	public const int defaultInterlining = 2;
	public const int indentWidth = 16;
	public const int layoutSpaces = 106;
}

public class EditorUtils
{
	public static Rect PropertyList(Rect position, SerializedProperty property)
	{
		if (!property.isArray) throw new System.Exception("[PropertyList] Property argument must be an array");

		float totalHeight = 0;

		EditorGUI.indentLevel++;
		if (property.arraySize == 0)
		{
			Rect addButtonRect = new Rect(position.x + EditorGUI.indentLevel * K.indentWidth, position.y, 100, K.defaultPropHeight);
			if (GUI.Button(addButtonRect, new GUIContent("Add property")))
			{
				property.InsertArrayElementAtIndex(0);
			}
			totalHeight = K.defaultPropHeight;
		}
		else
		{
			for (int i = 0; i < property.arraySize; i++)
			{
				SerializedProperty prop = property.GetArrayElementAtIndex(i);
				float propHeight = EditorGUI.GetPropertyHeight(prop);
				Rect propRect = new Rect(position.x, position.y + totalHeight, position.width-40, propHeight);
				EditorGUI.PropertyField(propRect, prop);

				totalHeight += propHeight+K.defaultInterlining;
				
				Rect addButtonRect = new Rect(propRect.x + propRect.width , propRect.y, 20, K.defaultPropHeight);
				if (GUI.Button(addButtonRect, new GUIContent("+")))
				{
					property.InsertArrayElementAtIndex(i);
					for (int j = property.arraySize - 1; j > i + 1; j--)
					{
						property.GetArrayElementAtIndex(j).isExpanded = property.GetArrayElementAtIndex(j - 1).isExpanded;
					}
					property.GetArrayElementAtIndex(i + 1).isExpanded = false;
				}
				Rect removeButtonRect = new Rect(propRect.x + propRect.width +20, propRect.y, 20, K.defaultPropHeight);
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

		return new Rect(position.x,position.y,position.width,totalHeight);
	}

	public static float GetPropertyListHeight(SerializedProperty property)
	{
		if (!property.isArray) throw new System.Exception("[GetPropertyListHeight] Property argument must be an array");

		if (property.arraySize == 0) return K.defaultPropHeight + K.defaultInterlining;
		else
		{
			float totalHeight=0;
			for (int i = 0; i < property.arraySize; i++)
			{
				totalHeight += EditorGUI.GetPropertyHeight(property.GetArrayElementAtIndex(i)) + K.defaultInterlining;
			}
			return totalHeight;
		}
	}
}

[CustomPropertyDrawer(typeof(Operation))]
public class OperationDrawer : PropertyDrawer
{

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), GUIContent.none);

		int indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		Rect keyRect = new Rect(position.x, position.y, 100, K.defaultPropHeight);
		Rect operationRect = new Rect(position.x + 110, position.y, 100, K.defaultPropHeight);
		Rect valueRect = new Rect(position.x + 220, position.y, 100, K.defaultPropHeight);

		SerializedProperty operationType = property.FindPropertyRelative(nameof(Operation.operationType));

		switch ((OperationType)operationType.intValue)
		{
			case OperationType.Set:
			case OperationType.Add:
				EditorGUI.PropertyField(keyRect, property.FindPropertyRelative(nameof(Operation.key)), GUIContent.none);
				break;
		}
		EditorGUI.PropertyField(operationRect, operationType, GUIContent.none);
		switch ((OperationType)operationType.intValue)
		{
			case OperationType.Set:
			case OperationType.Add:
				EditorGUI.PropertyField(valueRect, property.FindPropertyRelative(nameof(Operation.value)), GUIContent.none);
				break;
			case OperationType.GoToMap:
			case OperationType.InitiateFight:
			case OperationType.PlayGameEvent:
			case OperationType.AddItem:
			case OperationType.RemoveItem:
				EditorGUI.PropertyField(valueRect, property.FindPropertyRelative(nameof(Operation.reference)), GUIContent.none);
				break;
			case OperationType.GoToCell:
				EditorGUI.PropertyField(valueRect, property.FindPropertyRelative(nameof(Operation.position)), GUIContent.none);
				break;
		}
		
		EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty();
	}
}

[CustomPropertyDrawer(typeof(Condition))]
public class ConditionDrawer : PropertyDrawer
{

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), GUIContent.none);

		int indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		Rect keyRect = new Rect(position.x, position.y, 100, K.defaultPropHeight);
		Rect conditionRect = new Rect(position.x + 110, position.y, 100, K.defaultPropHeight);
		Rect valueRect = new Rect(position.x + 220, position.y, 100, K.defaultPropHeight);

		SerializedProperty conditionType = property.FindPropertyRelative(nameof(Condition.conditionType));

		switch ((ConditionType)conditionType.intValue)
		{
			case ConditionType.RandomChance:
				break;
			default:
				EditorGUI.PropertyField(keyRect, property.FindPropertyRelative(nameof(Condition.key)), GUIContent.none);
				break;
		}
		
		EditorGUI.PropertyField(conditionRect, conditionType, GUIContent.none);

		switch ((ConditionType)conditionType.intValue)
		{
			case ConditionType.Exists:
			case ConditionType.DoesNotExist:
				break;
			default:
				EditorGUI.PropertyField(valueRect, property.FindPropertyRelative(nameof(Condition.value)), GUIContent.none);
				break;
		}
		

		EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty();
	}
}

[CustomPropertyDrawer(typeof(Choice))]
public class ChoiceDrawer : PropertyDrawer
{
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return
			EditorUtils.GetPropertyListHeight(property.FindPropertyRelative(nameof(Choice.conditions))) +
			K.defaultInterlining +
			K.defaultPropHeight +
			K.defaultInterlining +
			EditorUtils.GetPropertyListHeight(property.FindPropertyRelative(nameof(Choice.operations)));
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), GUIContent.none);

		float top = position.y;

		int indentLevel = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		position.y += EditorUtils.PropertyList(position, property.FindPropertyRelative(nameof(Choice.conditions))).height;

		EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, K.defaultPropHeight), property.FindPropertyRelative(nameof(Choice.text)), GUIContent.none);
		position.y += 18;

		position.y += EditorUtils.PropertyList(position, property.FindPropertyRelative(nameof(Choice.operations))).height;

		EditorGUI.indentLevel = indentLevel;

		Color defaultColor = GUI.color;
		GUI.color = new Color(1.0f, 0.2f, 0.9f, 0.3f);
		GUI.Box(new Rect(position.x, top, position.width, position.y - top), GUIContent.none);
		GUI.color = defaultColor;

		EditorGUI.EndProperty();
	}
}

[CustomPropertyDrawer(typeof(Paragraph))]
public class GameEventParagraphDrawer : PropertyDrawer
{
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{

		SerializedProperty conditions = property.FindPropertyRelative(nameof(Paragraph.conditions));
		SerializedProperty operations = property.FindPropertyRelative(nameof(Paragraph.operations));
		SerializedProperty choices = property.FindPropertyRelative(nameof(Paragraph.choices));

		if (!property.isExpanded) return K.defaultPropHeight;
		return
			20 + //title
			20 + //foldout
			(conditions.isExpanded ?
				EditorUtils.GetPropertyListHeight(conditions) :
				0) +
			100 + //text
			20 + //foldout
			(operations.isExpanded ?
				EditorUtils.GetPropertyListHeight(operations) :
				0) +
			20 + //foldout
			(choices.isExpanded ?
				EditorUtils.GetPropertyListHeight(choices) :
				0);

	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive),GUIContent.none);

		float top = position.y;

		Rect paragraphFoldoutRect = new Rect(position.x, position.y, 20, K.defaultPropHeight);
		property.isExpanded = EditorGUI.Foldout(paragraphFoldoutRect, property.isExpanded, GUIContent.none);
		Rect titleRect = new Rect(position.x, position.y, position.width, K.defaultPropHeight);
		EditorGUI.LabelField(titleRect, new GUIContent(property.FindPropertyRelative(nameof(Paragraph.RawText)).stringValue), EditorStyles.boldLabel);
		position.y += 20;

		if (property.isExpanded)
		{

			SerializedProperty conditions = property.FindPropertyRelative(nameof(Paragraph.conditions));
			//conditions.isExpanded = EditorGUILayout.Foldout(conditions.isExpanded, new GUIContent("Conditions"));
			Rect foldoutRect = new Rect(position.x, position.y, position.width, 20);
			conditions.isExpanded = EditorGUI.Foldout(foldoutRect, conditions.isExpanded, new GUIContent($"Conditions ({conditions.arraySize})"));
			position.y += 20;
			if (conditions.isExpanded)
			{
				position.y += EditorUtils.PropertyList(position, conditions).height;

			}


			EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, 100), property.FindPropertyRelative(nameof(Paragraph.RawText)));
			position.y += 100;

			SerializedProperty operations = property.FindPropertyRelative(nameof(Paragraph.operations));
			foldoutRect = new Rect(position.x, position.y, position.width, 20);
			operations.isExpanded = EditorGUI.Foldout(foldoutRect, operations.isExpanded, new GUIContent($"Operations ({operations.arraySize})"));
			position.y += 20;
			if (operations.isExpanded)
			{
				position.y += EditorUtils.PropertyList(position, operations).height;
			}
			SerializedProperty choices = property.FindPropertyRelative(nameof(Paragraph.choices));
			foldoutRect = new Rect(position.x, position.y, position.width, 20);
			choices.isExpanded = EditorGUI.Foldout(foldoutRect, choices.isExpanded, new GUIContent($"Choices ({choices.arraySize})"));
			position.y += 20;
			if (choices.isExpanded)
			{
				position.y += EditorUtils.PropertyList(position, choices).height;
			}

		}
		Color defaultColor = GUI.color;
		GUI.color = new Color(1.0f, 1.0f, 0.9f, 0.3f);
		GUI.Box(new Rect(position.x, top, position.width, position.y - top), GUIContent.none);
		GUI.color = defaultColor;

		EditorGUI.EndProperty();
	}
}

[CustomEditor(typeof(GameEvent))]
public class GameEventEditor : Editor
{
	SerializedProperty paragraphs;
	public Vector2 scrollPosition = Vector2.zero;

	private void OnEnable()
	{
		paragraphs = serializedObject.FindProperty(nameof(GameEvent.paragraphs));
	}

	public override void OnInspectorGUI()
	{
		for (int i = 0; i < K.layoutSpaces; ++i) EditorGUILayout.Space();

		float totalHeight = EditorUtils.GetPropertyListHeight(paragraphs)+20;
		
		Rect position   = new Rect(0, 0, EditorGUIUtility.currentViewWidth-20, totalHeight);
		Rect scrollRect = new Rect(0, 0, EditorGUIUtility.currentViewWidth, 600);
		Rect scrollView = new Rect(0, 0, EditorGUIUtility.currentViewWidth-20, totalHeight);

		scrollPosition = GUI.BeginScrollView(scrollRect, scrollPosition, scrollView);

		EditorGUI.BeginChangeCheck();

		position.y += EditorUtils.PropertyList(position, paragraphs).height;

		GUI.EndScrollView();

		if (EditorGUI.EndChangeCheck())
		{
			serializedObject.ApplyModifiedProperties();
		}
		

	}
}