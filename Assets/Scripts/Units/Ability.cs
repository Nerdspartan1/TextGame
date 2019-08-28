using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "ScriptableObjects/Ability")]
public class Ability : ScriptableObject
{
	public enum AbilityType
	{
		Heal,
		Damage,
	}

	public string Name;
	public int Value;
	public AbilityType Type;

}
