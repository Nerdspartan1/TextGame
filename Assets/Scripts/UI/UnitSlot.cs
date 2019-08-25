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

	public void UpdateSlot()
	{
		Name.text = Unit.Name;
		Level.text = $"Lvl. {Unit.Level}";

		LifeBar.Value = (float)Unit.Hp;
		LifeBar.MaxValue = (float)Unit.MaxHp;
		LifeBar.UpdateBar();
	}

	#region Character Panel
	public void OpenCharacterPanel()
	{
		if (Unit is Character character)
		{
			GameManager.Instance.CharacterPanel.SetActive(true);
			GameManager.Instance.CharacterPanel.GetComponentInChildren<CharacterPanel>().Character = character;
		}
	}
	#endregion
}
