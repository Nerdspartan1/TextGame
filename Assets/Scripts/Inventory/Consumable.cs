using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName ="Consumable",menuName ="ScriptableObjects/Item/Consumable")]
public class Consumable : Item
{


	public enum ConsumableType
	{
		Heal,
	}

	public int UseValue;
	public ConsumableType Type;

	public void Use(Unit target)
	{
		switch (Type)
		{
			case ConsumableType.Heal:
				target.Heal(UseValue);
				break;
		}
		Inventory.Instance.Remove(this);
	}
}
