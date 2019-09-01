using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class AbilitySlot : MonoBehaviour
{
	private Ability Ability;
	private Character Actor;

	public Text AbilityName;

	public AbilityOptionPanel OptionPanel;

	public void SetAbility(Ability ability, Character actor)
	{
		Actor = actor;
		Ability = ability;
		AbilityName.text = Ability.Name;

		OptionPanel.Ability = Ability;
		OptionPanel.Actor = Actor;
		OptionPanel.UpdateUI();
	}

	public void OpenOptions()
	{
		if (Ability)
		{
			OptionPanel.gameObject.SetActive(true);
		}
	}

	public void CloseOptions()
	{
		OptionPanel.gameObject.SetActive(false);
	}
}
