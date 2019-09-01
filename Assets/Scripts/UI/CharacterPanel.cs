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
	public Transform AbilityPanel;
	public GameObject AbilitySlotPrefab;
	public GameObject AbilityMaskingPanel;

	private AttributeBar[] AttributeBars;

	public void Start()
	{
		AttributeBars = AttributePanel.GetComponentsInChildren<AttributeBar>();
		foreach(var bar in AttributeBars)
		{
			bar.LevelUpButton.onClick.AddListener( delegate {
				(Unit as Character).LevelUpAttribute(bar.Attribute);
				UpdateUI();
			});
		}
		WeaponSlot.AllowedItemType = typeof(Weapon);
		CloseCharacterPanel();
	}

	public void SetUnit(Unit unit)
	{
		Unit = unit;
		UpdateUI();
	}

	public void UpdateUI()
	{
		Character character = Unit as Character;

		NameText.text = Unit.Name;
		LevelText.text = $"Level {Unit.Level}";
		DescriptionText.text = Unit.Description;

		HealthBar.Value = Unit.Hp;
		HealthBar.MaxValue = Unit.MaxHp;
		HealthBar.UpdateBar();

		FocusBar.Value = Unit.Focus;
		FocusBar.MaxValue = Unit.MaxFocus;
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

			AbilityPanel.gameObject.SetActive(true);
			foreach (Transform child in AbilityPanel)
			{
				Destroy(child.gameObject);
			}
			foreach(Ability ability in Unit.Abilities)
			{
				var abilitySlot = Instantiate(AbilitySlotPrefab, AbilityPanel).GetComponent<AbilitySlot>();
				abilitySlot.SetAbility(ability, character);
				abilitySlot.CloseOptions();
			}
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
			AbilityPanel.gameObject.SetActive(false);
		}
	}

	public void UpdateEquipment()
	{
		(Unit as Character).Weapon = (Weapon)WeaponSlot.Item;
		UpdateUI();
	}

	public void CloseCharacterPanel()
	{
		GameManager.Instance.CharacterWindow.SetActive(false);
	}

}
