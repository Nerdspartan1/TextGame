using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class Map
{
	private List<List<GameEvent>> gameLocations = new List<List<GameEvent>>();

	string mapName;
	string path;

	/// <summary>
	/// Va lire le fichier "Assets/Text/Maps/" + mapName + ".txt"
	/// </summary>
	public Map(string mapName)
	{
		this.mapName = mapName;
		path = "Assets/Text/Maps/" + mapName + ".txt";
	}

	public GameEvent this[int i,int j]
	{
		get { return gameLocations[i][j];  }
	}

	/// <summary>
	/// Cherche le fichier dans Maps/mapName/
	/// </summary>
	/// <param name="locationName"></param>
	/// <returns></returns>
	public GameEvent Find(string locationName)
	{
		foreach(List<GameEvent> row in gameLocations)
		{
			foreach(GameEvent ge in row)
			{
				if(ge!= null && ge.path == "Assets/Text/Maps/"+mapName+"/"+locationName+".txt")
				{
					return ge;
				}
			}
		}
		return null;
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
				if (ge != null && ge.path == "Assets/Text/Maps/" + mapName + "/" + locationName + ".txt")
				{
					return new Vector2(i, j);
				}
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

	public void Load()
	{
		StreamReader reader = new StreamReader(path);
		List<GameEvent> row = new List<GameEvent>();

		string name = "";
		bool endOfStream = false;
		char c = '%';
		while (!endOfStream || name!=""  )
		{
			if (!endOfStream)
			{
				c = (char)reader.Read();
				endOfStream = reader.EndOfStream;
			}
			else //Si on est en fin de stream et que le nom n'est pas vide: on fait comme si il y avait un retour à la ligne
			{
				c = '\n';
			}
			/*
			if (c == '\t') Debug.Log("Tabulation");
			else if (c == '\n') Debug.Log("Saut de ligne");
			else Debug.Log(c);
			*/
			if (c == '\n' || c == '\t')
			{
				GameEvent ge;
				if (name != "0")
				{
					ge = new GameEvent("Maps/"+ this.mapName + "/" + name);
					ge.IsMapLocation = true;
					Debug.Log("Ajout de " + name);
				}
				else
				{
					ge = null;
					Debug.Log("Ajout de null");
				}

				row.Add(ge);
				
				name = "";
				if(c == '\n')
				{
					Debug.Log("Ajout de row de taille " + row.Count);
					gameLocations.Add(row);
					row = new List<GameEvent>();
				}
			}
			else if(c > 32)
			{
				name += c;
			}
		}
	}
}