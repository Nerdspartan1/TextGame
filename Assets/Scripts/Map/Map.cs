using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

[CreateAssetMenu(fileName = "Map", menuName = "ScriptableObjects/Map")]
public class Map : ScriptableObject
{
	[SerializeField]
	private List<Location> locations = new List<Location>();

	[SerializeField]
	private int width = 1;
	public int Width { get { return width; } }

	[SerializeField]
	private int height = 1;
	public int Height { get { return height; } }

	public Location this[Vector2Int v]{
		get {
			if (v.x >= width || v.x < 0 ||
				v.y >= height|| v.y < 0)
			{
				Debug.LogError($"[Map] {v} is out of bounds (width={width},height={height}) ");
				return null;
			}
			return locations[v.y * width + v.x];
		}
	}
}