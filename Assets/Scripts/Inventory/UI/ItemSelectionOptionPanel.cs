using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ItemSelectionOptionPanel : OptionPanel
{

	public ItemSlot ItemSlot;

	public override void UpdateUI()
	{
		ClearButtons();

		if (ItemSlot.Item == null) return;

		if (ItemSlot.Item is Consumable c)
		{
			foreach (Character character in GameManager.Instance.PlayerTeam) {
				AddButton($"Use on {character.Name}", delegate { c.Use(character); });
			}
		}
		AddButton("Discard", delegate { Inventory.Instance.Remove(ItemSlot.Item); });
	}

}
