using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Location.RandomOperation))]
public class RandomOperationDrawer : PropertyDrawer
{
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return 18 + EditorUtils.GetPropertyListHeight(property.FindPropertyRelative(nameof(Location.RandomOperation.operations)));
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), GUIContent.none);

		SerializedProperty operations = property.FindPropertyRelative(nameof(Location.RandomOperation.operations));
		SerializedProperty probability = property.FindPropertyRelative(nameof(Location.RandomOperation.probability));

		probability.floatValue = EditorGUI.Slider(new Rect(position.x, position.y, position.width, 16), "Probability", probability.floatValue, 0, 1);
		position.y += 18;

		position.y += EditorUtils.PropertyList(position, operations).height;

		EditorGUI.EndProperty();
	}
}

[CustomEditor(typeof(Map))]
public class MapEditor : Editor
{
	public Vector2 scrollPosition = Vector2.zero;
	SerializedProperty locations;
	SerializedProperty width;
	SerializedProperty height;

	Vector2 buttonSize = new Vector2(40, 16);

	private void OnEnable()
	{
		locations = serializedObject.FindProperty("locations");
		width = serializedObject.FindProperty("width");
		height = serializedObject.FindProperty("height");
	}

	
	public override void OnInspectorGUI()
	{
		Rect position = new Rect(0, 0, EditorGUIUtility.currentViewWidth, 0);

		for(int i = 0; i < K.layoutSpaces; ++i) EditorGUILayout.Space();

		EditorGUI.BeginChangeCheck();

		EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, 16), width, new GUIContent("Width"));
		position.y += 18;
		EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, 16), height, new GUIContent("Height"));
		position.y += 18;

		Rect scrollView = new Rect(0, 0, (buttonSize.x+5)*width.intValue, (buttonSize.y + 5) * height.intValue);
		Rect scrollRect = new Rect(0, 0, EditorGUIUtility.currentViewWidth, 600);

		scrollPosition = GUI.BeginScrollView(scrollRect, scrollPosition, scrollView);

		for (int i = 0; i < locations.arraySize; ++i)
		{
			int u = i % width.intValue;
			int v = i / width.intValue;
			
			EditorGUI.PropertyField(new Rect(position.x + u * (buttonSize.x + 5), position.y + v * (buttonSize.y + 5), buttonSize.x, buttonSize.y), locations.GetArrayElementAtIndex(i),GUIContent.none);
		}

		if (EditorGUI.EndChangeCheck())
		{
			locations.arraySize = width.intValue * height.intValue;
			serializedObject.ApplyModifiedProperties();
		}

		GUI.EndScrollView();

	}
}
