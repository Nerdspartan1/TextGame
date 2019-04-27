using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class Map
{
	private List<List<GameEvent>> gameLocations = new List<List<GameEvent>>();

	string mapName;

	/// <summary>
	/// Va lire le fichier "Assets/Text/maps/" + mapName + ".txt"
	/// </summary>
	public Map(string mapName)
	{
		this.mapName = mapName;

	}

	public GameEvent this[int i,int j]
	{
		get { return gameLocations[i][j];  }
	}


	public Vector2 GetPosition(string locationName)
	{
		int i = 0;
		int j = 0;
		foreach (List<GameEvent> row in gameLocations)
		{
			j = 0;
			foreach (GameEvent ge in row)
			{
				/*if (ge != null && ge.path == "Assets/Text/maps/" + mapName + "/" + locationName + ".txt")
				{
					return new Vector2(i, j);
				}*/
				j++;
			}
			i++;
		}
		return new Vector2(-1, -1) ;
	}

	public int Width
	{
		get { return gameLocations[0].Count; }
	}

	public int Height
	{
		get { return gameLocations.Count; }
	}

}