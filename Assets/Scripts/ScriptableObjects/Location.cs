using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Location", menuName ="ScriptableObjects/Location")]
public class Location : ScriptableObject
{
	[System.Serializable]
	public struct RandomEvent
	{
		public GameEvent gameEvent;
		[Range(0,1)]
		public float probability;
	}

	[SerializeField]
	public Paragraph description;
	[SerializeField]
	public List<RandomEvent> randomEvents;

	public GameEvent GetRandomEvent()
	{
		float r = Random.Range(0f, (float)randomEvents.Count);
		float probOffset = 0f;
		foreach(RandomEvent rev in randomEvents)
		{
			if (r < probOffset + rev.probability) return rev.gameEvent;
			else probOffset += rev.probability;
		}
		return null;
	}


}
