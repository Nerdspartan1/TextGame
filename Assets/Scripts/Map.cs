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
	private int width;
	public int Width { get { return width; } }

	[SerializeField]
	private int height;
	public int Height { get { return height; } }

	public Location this[int u, int v]{
		get {
			if (u >= width || u < 0 ||
				v >= height|| v < 0)
			{
				Debug.LogError($"[Map] ({u},{v}) is out of bounds (width={width},height={height}) ");
				return null;
			}
			return locations[v * width + u];
		}
	}
}