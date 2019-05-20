using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Location", menuName ="ScriptableObjects/Location")]
public class Location : ScriptableObject
{
	[SerializeField]
	public Paragraph description;
}
