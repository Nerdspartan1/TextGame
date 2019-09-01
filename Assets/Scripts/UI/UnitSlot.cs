using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSlot : MonoBehaviour
{
	public Unit Unit;

	public Text Name;
	public Text Level;
	public Text Hp;
	public StatBar LifeBar;
	public Text Focus;
	public StatBar FocusBar;
	public GameObject LevelUpSymbol;

	public void UpdateSlot()
	{
		Name.text = Unit.Name;
		Level.text = $"Lvl. {Unit.Level}";

		LifeBar.Value = (float)Unit.Hp;
		LifeBar.MaxValue = (float)Unit.MaxHp;
		LifeBar.UpdateBar();

		FocusBar.Value = (float)Unit.Focus;
		FocusBar.MaxValue = (float)Unit.MaxFocus;
		FocusBar.UpdateBar();

		if (Unit is Character character)
		{
			LevelUpSymbol.SetActive(character.AvailableAttributePoints > 0);
		}
	}

	#region Character Panel
	public void OpenCharacterPanel()
	{
		GameManager.Instance.CharacterWindow.SetActive(true);
		GameManager.Instance.CharacterWindow.GetComponentInChildren<CharacterPanel>().SetUnit(Unit);
	}
	#endregion
}
