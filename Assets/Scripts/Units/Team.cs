using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Team", menuName = "ScriptableObjects/Unit/Team", order = 1)]
public class Team : ScriptableObject, ICollection<Unit>
{
	public List<Unit> Units = new List<Unit>();

	public void InstantiateUnits()
	{
		for(int i = 0; i < Units.Count; ++i)
		{
			Units[i] = Instantiate(Units[i]);
		}
		MakeNamesUnique();
	}

	public Unit this[int i]
	{
		get => Units[i];
	}

	public void MakeNamesUnique()
	{
		Dictionary<string, int> usedNameCounts = new Dictionary<string, int>();
		Dictionary<string, int> usedNameNumber = new Dictionary<string, int>();
		foreach (var unit in Units) //1st pass : count the names
		{
			if (!usedNameCounts.ContainsKey(unit.Name))
			{
				usedNameCounts.Add(unit.Name, 1);
				usedNameNumber.Add(unit.Name, 0);
			}
			else
				usedNameCounts[unit.Name]++;
		}
		
		foreach (var unit in Units) //2nd pass : rename
		{
			if (usedNameCounts[unit.Name] > 1) { //rename only if there are 2 identical names or more
				unit.Name += $" {++usedNameNumber[unit.Name]}";
			}
		}

	}

	public int Count { get => Units.Count; }

	public bool IsReadOnly => ((ICollection<Unit>)Units).IsReadOnly;

	public void Add(Unit item)
	{
		((ICollection<Unit>)Units).Add(item);
	}

	public void Clear()
	{
		((ICollection<Unit>)Units).Clear();
	}

	public bool Contains(Unit item)
	{
		return ((ICollection<Unit>)Units).Contains(item);
	}

	public void CopyTo(Unit[] array, int arrayIndex)
	{
		((ICollection<Unit>)Units).CopyTo(array, arrayIndex);
	}

	public IEnumerator<Unit> GetEnumerator()
	{
		return Units.GetEnumerator();
	}

	public bool Remove(Unit item)
	{
		return ((ICollection<Unit>)Units).Remove(item);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}

