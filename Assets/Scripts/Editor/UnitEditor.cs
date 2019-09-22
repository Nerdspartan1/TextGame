using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;

[CustomEditor(typeof(Unit),true)]
public class UnitEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		if(GUILayout.Button("Calculate stats from attributes"))
		{
			((Unit)serializedObject.targetObject).CalculateStatsFromAttributes(true);
		}
	}
}
