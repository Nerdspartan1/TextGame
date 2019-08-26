using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPanel : MonoBehaviour
{
	public Unit Unit;
	
	[Header("References")]
	public Text NameText;
	public Text LevelText;
	public Text DescriptionText;
	public StatBar XPBar;
	public StatBar HealthBar;
	public StatBar FocusBar;
	public Transform AttributePanel;
	public ItemSlot WeaponSlot;

	private AttributeBar[] AttributeBars;

	public void Start()
	{
		AttributeBars = AttributePanel.GetComponentsInChildren<AttributeBar>();
		foreach(var bar in AttributeBars)
		{
			bar.LevelUpButton.onClick.AddListener( delegate { (Unit as Character).LevelUpAttribute(bar.Attribute); });
		}
		WeaponSlot.AllowedItemType = typeof(Weapon);
	}

	public void Update()
	{
		UpdateUI();
	}

	public void SetUnit(Unit unit)
	{
		if (unit == Unit) return;
		Unit = unit;
		UpdateUI();
	}

	public void UpdateUI()
	{
		if (!Unit)
		{
			CloseCharacterPanel();
			return;
		}

		Character character = Unit as Character;

		NameText.text = Unit.Name;
		LevelText.text = $"Level {Unit.Level}";
		DescriptionText.text = Unit.Description;

		HealthBar.Value = Unit.Hp;
		HealthBar.MaxValue = Unit.MaxHp;
		HealthBar.UpdateBar();

		FocusBar.Value = 0;
		FocusBar.MaxValue = 0;
		FocusBar.UpdateBar();

		if (character)
		{
			XPBar.gameObject.SetActive(true);
			XPBar.Value = character.XP - Character.XPLevel(character.Level);
			XPBar.MaxValue = Character.XPLevel(character.Level + 1) - Character.XPLevel(character.Level);
			XPBar.UpdateBar();

			foreach (var bar in AttributeBars)
			{
				bar.Value = Unit.GetAttribute(bar.Attribute);
				bar.MaxValue = 100;
				bar.UpdateBar();

				bar.LevelUpButton.gameObject.SetActive(true);
				bar.LevelUpButton.interactable = (character.AvailableAttributePoints > 0);
			}


			WeaponSlot.gameObject.SetActive(true);
			WeaponSlot.SetItem(character.Weapon);
		}
		else
		{
			XPBar.gameObject.SetActive(false);

			foreach (var bar in AttributeBars)
			{
				bar.Value = Unit.GetAttribute(bar.Attribute);
				bar.MaxValue = 100;
				bar.UpdateBar();

				bar.LevelUpButton.gameObject.SetActive(false);
			}

			WeaponSlot.gameObject.SetActive(false);
		}
	}

	public void LevelUpCharacter(Attribute attribute)
	{
		(Unit as Character).LevelUpAttribute(attribute);
	}

	public void UpdateEquipment()
	{
		(Unit as Character).Weapon = (Weapon)WeaponSlot.Item;
	}

	public void CloseCharacterPanel()
	{
		GameManager.Instance.CharacterPanel.SetActive(false);
	}

}
