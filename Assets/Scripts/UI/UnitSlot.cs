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
	public RectTransform LifeBar;

	public void UpdateSlot()
	{
		Name.text = Unit.Name;
		Level.text = $"Lvl. {Unit.Level}";
		Hp.text = $"{Unit.Hp}/{Unit.MaxHp}";

		float hpRatio = (float)Unit.Hp / (float)Unit.MaxHp;
		LifeBar.localScale = new Vector3(hpRatio > 0 ? hpRatio : 0, 1, 1);
	}
}
