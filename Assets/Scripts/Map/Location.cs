using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Location", menuName ="ScriptableObjects/Location")]
public class Location : GameEvent
{
	public EnemyEncounterTable EncounterTable;
	public Sprite Icon;
	public Color Color = new Color(162,194,255);
}
