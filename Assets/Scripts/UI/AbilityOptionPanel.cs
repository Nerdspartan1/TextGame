using UnityEngine;
using System.Collections.Generic;

public class AbilityOptionPanel : OptionPanel
{
	public Character Actor;
	public Ability Ability;

	public override void UpdateUI()
	{
		ClearButtons();

		if (Ability.AbilityType != AbilityType.Heal) return;

		if (Ability.TargettingType == TargettingType.Single) {
			foreach (var teammate in GameManager.Instance.PlayerTeam)
			{
				var button = AddButton($"Use on {teammate.Name}", delegate
				{
					Actor.UseAbility(new List<Unit>() { teammate }, Ability, out _);
				});
				var buttonRect = button.GetComponent<RectTransform>();
				buttonRect.sizeDelta = new Vector2(buttonRect.sizeDelta.x, 20);
			}
		}
		else
		{
			AddButton($"Use on all", delegate
			{
				Actor.UseAbility(GameManager.Instance.PlayerTeam, Ability, out _);
			});
		}
	}
}
