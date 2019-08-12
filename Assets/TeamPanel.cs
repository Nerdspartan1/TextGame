using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamPanel : MonoBehaviour
{
	public Team Team;
	public GameObject CharacterSlotPrefab;

	private CharacterSlot[] characterSlots;
	
	public void UpdateSlots()
	{
		foreach(var cs in characterSlots)
		{
			cs.UpdateSlot();
		}
	}

	//to be called when team changes
	public void RebuildPanel()
	{
		foreach(Transform child in transform)
		{
			Destroy(child.gameObject);
		}
		var cslist = new List<CharacterSlot>();
		foreach(Character c in Team)
		{
			var cs = Instantiate(CharacterSlotPrefab, transform).GetComponent<CharacterSlot>();
			cs.Character = c;
			cslist.Add(cs);
		}
		characterSlots = cslist.ToArray();
	}
}
