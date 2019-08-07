using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Team", menuName = "ScriptableObjects/Unit/Team", order = 1)]
public class Team : ScriptableObject, ICollection<Unit>
{
	public List<Unit> Units = new List<Unit>();

	public Unit this[int i]
	{
		get => Units[i];
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

