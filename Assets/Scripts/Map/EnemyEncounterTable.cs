using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EncounterTable", menuName = "ScriptableObjects/EncounterTable")]
public class EnemyEncounterTable : ScriptableObject
{
	[System.Serializable]
	public class Encounter{
		[Range(0,1)]
		public float occurenceProbability;
		[Range(0,1)]
		public float groupPresenceProbability;
		public List<Enemy> enemies;
	}

	public List<Encounter> Encounters;

	public Team GetEncounter()
	{
		foreach(var enc in Encounters)
		{
			if (enc.enemies.Count == 0) throw new System.Exception("Enemy team empty");
		}

		float r = Random.Range(0f, Encounters.Count);
		float probabilityOffset = 0f;
		Encounter occuringEncounter = null;
		foreach (var encounter in Encounters)
		{
			if (r < probabilityOffset + encounter.occurenceProbability)
			{
				occuringEncounter = encounter;
			}
			else probabilityOffset += encounter.occurenceProbability;
		}

		if (occuringEncounter == null) return null;
		else
		{
			Team enemyTeam = ScriptableObject.CreateInstance<Team>();
			foreach(var enemy in occuringEncounter.enemies)
				if(Random.value < occuringEncounter.groupPresenceProbability)
					enemyTeam.Add(enemy);

			if (enemyTeam.Count == 0) //no empty team allowed
				enemyTeam.Add(occuringEncounter.enemies[0]);

			return enemyTeam;
		}
	}
}
