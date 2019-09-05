using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class SaveManager : MonoBehaviour
{

	public struct ValuePair
	{
		public string Key;
		public string Value;
	}

	public class SavedGame
	{
		public List<Unit> PlayerTeam;
		public List<int> EquippedWeapons;
		public List<string> Inventory;
		public int Money;
		public string Map;
		public Vector2Int Location;
		public List<ValuePair> Values;
	}

	private static XmlSerializer serializer = new XmlSerializer(typeof(SavedGame), new Type[] { typeof(Character)});

	public Transform SaveWindow;
	public Transform SavePanel;
	private SaveSlot[] saveSlots;

	private void Awake()
	{
		saveSlots = SavePanel.GetComponentsInChildren<SaveSlot>();
	}

	public void ToggleWindow()
	{
		SaveWindow.gameObject.SetActive(!SaveWindow.gameObject.activeInHierarchy);
		if (SaveWindow.gameObject.activeInHierarchy) UpdateUI();
	}

	public void UpdateUI()
	{
		int slotId = 0;
		foreach(SaveSlot slot in saveSlots)
		{
			slot.SetSlotId(slotId);
			slotId++;
		}
	}

	public static string GetSavePath(int slot)
	{
		return $"Saves/save{slot}.save";
	}

	public static void Save(int slot)
	{
		var savedGame = GameManager.Instance.Save();

		StreamWriter writer = new StreamWriter(GetSavePath(slot), false, System.Text.Encoding.UTF8);

		serializer.Serialize(writer, savedGame);

		writer.Close();
	}

	public static void Load(int slot)
	{
		StreamReader reader = new StreamReader(GetSavePath(slot), System.Text.Encoding.UTF8);

		var loadedGame = serializer.Deserialize(reader) as SavedGame;

		GameManager.Instance.Load(loadedGame);

		reader.Close();
	}

	public static void Delete(int slot)
	{
		File.Delete(GetSavePath(slot));
	}
}
