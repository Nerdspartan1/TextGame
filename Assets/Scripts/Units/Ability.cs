using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityType
{
	Heal,
	Damage,
}

public enum TargettingType
{
	Single,
	All
}

[CreateAssetMenu(fileName = "Ability", menuName = "ScriptableObjects/Ability")]
public class Ability : ScriptableObject
{
	public string Name;
	public int Value;
	public AbilityType AbilityType;
	public TargettingType TargettingType;

}
