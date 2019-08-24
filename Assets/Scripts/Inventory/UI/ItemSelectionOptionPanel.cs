using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ItemSelectionOptionPanel : MonoBehaviour
{
	public GameObject buttonPrefab;

	public ItemSlot ItemSlot;

	public void UpdateUI()
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

	private void AddButton(string name, UnityAction onClick)
	{
		Button b = Instantiate(buttonPrefab, transform).GetComponent<Button>();
		b.GetComponentInChildren<Text>().text = name;
		b.onClick.AddListener(onClick);
	}

	private void ClearButtons()
	{
		foreach (Transform child in transform)
		{
			Destroy(child.gameObject);
		}
	}
}
