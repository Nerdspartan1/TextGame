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
	public StatBar XPBar;
	public StatBar HealthBar;
	public StatBar FocusBar;

	public void Update()
	{
		if (!Character)
		{
			CloseCharacterPanel();
			return;
		}

		NameText.text = Character.Name;
		LevelText.text = $"Level {Character.Level}";

		XPBar.Value = Character.XP;
		XPBar.MaxValue = Character.NextLevelXP();
		XPBar.UpdateBar();
		HealthBar.Value = Character.Hp;
		HealthBar.MaxValue = Character.MaxHp;
		HealthBar.UpdateBar();
		FocusBar.Value = 0;
		FocusBar.MaxValue = 0;
		FocusBar.UpdateBar();
	}

	public void CloseCharacterPanel()
	{
		GameManager.Instance.CharacterPanel.SetActive(false);

	}
}
