using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamPanel : MonoBehaviour
{
	public Team Team { get; private set; }

	[Header("Prefabs")]
	public GameObject CharacterSlotPrefab;

	private UnitSlot[] unitSlots;

	public void UpdateSlots()
	{
		foreach(var slot in unitSlots)
		{
			slot.UpdateSlot();
		}
	}

	//to be called when team changes
	public void SetTeam(Team team)
	{
		Team = team;
		foreach(Transform child in transform)
		{
			Destroy(child.gameObject);
		}
		var slotList = new List<UnitSlot>();
		foreach(Unit u in Team)
		{
			var slot = Instantiate(CharacterSlotPrefab, transform).GetComponent<UnitSlot>();
			slot.Unit = u;
			slotList.Add(slot);
		}
		unitSlots = slotList.ToArray();
		UpdateSlots();
	}

}
