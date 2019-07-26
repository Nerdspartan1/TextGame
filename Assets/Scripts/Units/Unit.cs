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

[CreateAssetMenu(fileName = "Unit", menuName = "ScriptableObjects/Unit/Unit", order = 1)]
public abstract class Unit : ScriptableObject
{
	[Header("Identity")]
	public string Name;
	[TextArea(3,10)]
	public string Description;

	[Header("Attributes")]
	public ushort Strength;
	public ushort Speed;
	public ushort Perception;
	public ushort Skill;
	public ushort Endurance;

	[Header("Stats")]
	public ushort Level;
	[SerializeField]
	private int maxHp;
	public int MaxHp {get => maxHp;}
	[SerializeField]
	private int hp;
	public int Hp {get => hp;}

	public virtual void Init()
	{
		hp = MaxHp;
		Debug.Log($"{Name} initialized.");
	}

	public void CalculateStatsFromAttributes()
	{
		maxHp = (int)(20 + Endurance * 5 + Strength * 2);
		if (hp > maxHp) hp = maxHp;
	}

	public bool IsDead{ get => Hp <= 0; }

	public abstract void Attack(Unit other);

	public void TakeDamage(int dmg)
	{
		hp -= dmg;

		if(hp <= 0)
		{
			Die();
		}
	}

	public void Heal(int heal)
	{
		hp += heal;
		if(hp > MaxHp)
		{
			hp = MaxHp;
		}
	}

	public abstract void Die();
}
