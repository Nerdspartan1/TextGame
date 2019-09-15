using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CharacterPanel : MonoBehaviour
{
	public Unit Unit;
	
	[Header("References")]
	public Text NameText;
	public Text LevelText;
	public StatBar XPBar;
	public Button AddToFightTeamButton;
	public StatBar HealthBar;
	public StatBar FocusBar;
	public Transform AttributePanel;
	public ItemSlot WeaponSlot;
	public Transform AbilityPanel;
	public GameObject AbilitySlotPrefab;
	public GameObject AbilityMaskingPanel;
	public Text DescriptionText;

	private AttributeBar[] AttributeBars;
	public bool LockCharacterSwap = false;

	public void Awake()
	{
		WeaponSlot.AllowedItemType = typeof(Weapon);
		AttributeBars = AttributePanel.GetComponentsInChildren<AttributeBar>();
		foreach (var bar in AttributeBars)
		{
			bar.LevelUpButton.onClick.AddListener(delegate {
				(Unit as Character).LevelUpAttribute(bar.Attribute);
				UpdateUI();
			});
		}
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

			AddToFightTeamButton.gameObject.SetActive(true);
			AddToFightTeamButton.onClick.RemoveAllListeners();

			AddToFightTeamButton.GetComponentInChildren<Text>().text = character.InFightTeam ? "Remove from fight team" : "Add to fight team";

			AddToFightTeamButton.interactable = !LockCharacterSwap && (character.InFightTeam ?
				(character.CanBeRemovedFromFightTeam) :
				(GameManager.Instance.PlayerTeam.Count(unit => (unit as Character).InFightTeam) < 4));

			if (AddToFightTeamButton.interactable)
				AddToFightTeamButton.onClick.AddListener(
					delegate
					{
						character.InFightTeam = !character.InFightTeam;
						GameManager.Instance.TeamPanel.UpdateSlots();
						UpdateUI();
					}
					);


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
			AddToFightTeamButton.gameObject.SetActive(false);

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
		(Unit as Character).Equip((Weapon)WeaponSlot.Item);
		UpdateUI();
	}

	public void CloseCharacterPanel()
	{
		GameManager.Instance.CharacterWindow.SetActive(false);
	}

}
