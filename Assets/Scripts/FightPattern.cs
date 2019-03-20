using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FightPattern", menuName = "ScriptableObjects/FightPattern", order = 1)]
public class FightPattern : ScriptableObject
{
	//List<Bullet> bullets = new List<Bullet>();
	//List<float> time = new List<float>();

	public float shootFrequency, bulletSpeed;
}
