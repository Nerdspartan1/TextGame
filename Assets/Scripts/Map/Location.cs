using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Location", menuName ="ScriptableObjects/Location")]
public class Location : ScriptableObject
{
	[System.Serializable]
	public struct RandomOperation
	{
		[Range(0, 1)]
		public float probability;
		public List<Operation> operations;

	}

	[SerializeField]
	public Paragraph description;
	[SerializeField]
	public List<RandomOperation> randomEvents;

	public List<Operation> GetRandomOperation()
	{
		float r = Random.Range(0f, (float)randomEvents.Count);
		float probOffset = 0f;
		foreach(RandomOperation rev in randomEvents)
		{
			if (r < probOffset + rev.probability) return rev.operations;
			else probOffset += rev.probability;
		}
		return new List<Operation>();
	}


}
