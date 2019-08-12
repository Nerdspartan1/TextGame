using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSlot : MonoBehaviour
{
	public Character Character;

	public Text Name;
	public Text Level;
	public Text Hp;
	public RectTransform LifeBar;
	private Vector2 lifeBarSize;

	public void Start()
	{
		lifeBarSize = LifeBar.sizeDelta;
	}

	public void UpdateSlot()
    {
		Name.text = Character.Name;
		Level.text = $"Lvl. {Character.Level}";
		Hp.text = $"{Character.Hp}/{Character.MaxHp}";
		LifeBar.sizeDelta = new Vector2(lifeBarSize.x * (float)Character.Hp / (float)Character.MaxHp, lifeBarSize.y);
	}
}
