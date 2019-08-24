using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamPanel : MonoBehaviour
{
	public Team Team;
	public GameObject CharacterSlotPrefab;

	private UnitSlot[] unitSlots;
	
	public void UpdateSlots()
	{
		foreach(var cs in unitSlots)
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
		var uslist = new List<UnitSlot>();
		foreach(Unit u in Team)
		{
			var us = Instantiate(CharacterSlotPrefab, transform).GetComponent<UnitSlot>();
			us.Unit = u;
			uslist.Add(us);
		}
		unitSlots = uslist.ToArray();
		UpdateSlots();
	}
}
