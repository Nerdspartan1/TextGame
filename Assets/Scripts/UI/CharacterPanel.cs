using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPanel : MonoBehaviour
{
	public Character Character;
	
	[Header("References")]
	public Text NameText;
	public Text LevelText;
	public Text DescriptionText;
	public StatBar XPBar;
	public StatBar HealthBar;
	public StatBar FocusBar;
	public ItemSlot WeaponSlot;

	public List<AttributeBar> AttributeBars;

	public void Start()
	{
		foreach(var bar in AttributeBars)
		{
			bar.LevelUpButton.onClick.AddListener( delegate { Character.LevelUpAttribute(bar.Attribute); });
		}
		WeaponSlot.AllowedItemType = typeof(Weapon);
	}

	public void Update()
	{
		if (!Character)
		{
			CloseCharacterPanel();
			return;
		}

		NameText.text = Character.Name;
		LevelText.text = $"Level {Character.Level}";
		DescriptionText.text = Character.Description;

		XPBar.Value = Character.XP - Character.XPLevel(Character.Level);
		XPBar.MaxValue = Character.XPLevel(Character.Level+1) - Character.XPLevel(Character.Level);
		XPBar.UpdateBar();
		HealthBar.Value = Character.Hp;
		HealthBar.MaxValue = Character.MaxHp;
		HealthBar.UpdateBar();
		FocusBar.Value = 0;
		FocusBar.MaxValue = 0;
		FocusBar.UpdateBar();

		foreach(var bar in AttributeBars)
		{
			bar.Value = Character.GetAttribute(bar.Attribute);
			bar.MaxValue = 100;
			bar.UpdateBar();
			bar.LevelUpButton.interactable = (Character.AvailableAttributePoints > 0);
		}

		WeaponSlot.SetItem(Character.Weapon);
	}

	public void LevelUpCharacter(Attribute attribute)
	{
		Character.LevelUpAttribute(attribute);
	}

	public void UpdateEquipment()
	{
		Character.Weapon = (Weapon)WeaponSlot.Item;
	}

	public void CloseCharacterPanel()
	{
		GameManager.Instance.CharacterPanel.SetActive(false);
	}

}
