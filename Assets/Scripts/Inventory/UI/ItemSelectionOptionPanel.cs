using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ItemSelectionOptionPanel : OptionPanel
{

	public ItemSlot ItemSlot;
	public bool Owned = true;

	public override void UpdateUI()
	{
		ClearButtons();

		if (ItemSlot.Item == null) return;

		if (Owned)
		{
			if (ItemSlot.Item is Consumable c)
			{
				foreach (Character character in GameManager.Instance.PlayerTeam)
				{
					AddButton($"Use on {character.Name}", delegate { c.Use(character); });
				}
			}
			if(Inventory.Instance.CanSellItems) AddButton($"Sell ({ItemSlot.Item.Value})", delegate {
				Inventory.Instance.Money += ItemSlot.Item.Value;
				Inventory.Instance.Remove(ItemSlot.Item);
			});
			else AddButton("Discard", delegate { Inventory.Instance.Remove(ItemSlot.Item); });
		}
		else
		{
			var button = AddButton($"Purchase ({ItemSlot.Item.Value})", delegate {
				if (Inventory.Instance.Money >= ItemSlot.Item.Value)
				{
					if(Inventory.Instance.Add(ItemSlot.Item))
						Inventory.Instance.Money -= ItemSlot.Item.Value;
				}
				
			});
			button.interactable = (Inventory.Instance.Money >= ItemSlot.Item.Value );
		}
	}

}
