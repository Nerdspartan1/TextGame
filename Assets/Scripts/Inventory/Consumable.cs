using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName ="Consumable",menuName ="ScriptableObjects/Item/Consumable")]
public class Consumable : Item
{
	public UnityEvent onConsumed;

	public void Use()
	{
		onConsumed.Invoke();
		Inventory.Instance.Remove(this);
	}
}
