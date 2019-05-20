using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

[CreateAssetMenu(fileName = "Map", menuName = "ScriptableObjects/Map")]
public class Map : ScriptableObject
{
	public List<Location> locations = new List<Location>();

	public int width, height;

}