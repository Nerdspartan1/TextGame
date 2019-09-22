using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/Item/Weapon", order = 1)]
public class Weapon : Item {

	[Header("Damage")]
	public int minDmg;
	public int maxDmg;

	public int MinimumStrength = 0;
	public int MinimumSkill = 0;
	public int MinimumIntelligence = 0;

	public float StrengthScale = 0;
	public float SkillScale = 0;
	public float IntelligenceScale = 0;

	public int GetDamage()
	{
		return Random.Range(minDmg, maxDmg + 1);
	}

}
