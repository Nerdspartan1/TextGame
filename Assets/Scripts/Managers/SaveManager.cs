using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
	public static SaveManager Instance;
	private void Awake()
	{
		Instance = this;
	}

	public class SavedGame
	{
		public List<Unit> PlayerTeam;
		public List<int> EquippedWeapons;
		public List<string> Inventory;
	}

	private XmlSerializer serializer = new XmlSerializer(typeof(SavedGame), new Type[] { typeof(Character)});

	public void Save(int slot)
	{
		var savedGame = GameManager.Instance.Save();

		StreamWriter writer = new StreamWriter($"save{slot}.dat", false, System.Text.Encoding.UTF8);

		serializer.Serialize(writer, savedGame);

		writer.Close();
	}

	public void Load(int slot)
	{
		StreamReader reader = new StreamReader($"save{slot}.dat", System.Text.Encoding.UTF8);

		var loadedGame = serializer.Deserialize(reader) as SavedGame;

		GameManager.Instance.Load(loadedGame);

		reader.Close();
	}
}
