using System.Collections.Generic;
using UnityEngine;

public class WeightedRandomBag<T>
{
	public struct Entry
	{
		public float accumulatedWeight;

		public float weight;

		public T item;
	}

	private List<Entry> entries = new List<Entry>();

	private float totalWeight;

	private Entry lastPickedEntry;

	public int Count => entries.Count;

	public void AddEntry(T item, float weight)
	{
		totalWeight += weight;
		entries.Add(new Entry
		{
			item = item,
			accumulatedWeight = totalWeight,
			weight = weight
		});
	}

	public T Choose()
	{
		float num = Random.value * totalWeight;
		foreach (Entry entry in entries)
		{
			if (entry.accumulatedWeight >= num)
			{
				lastPickedEntry = entry;
				return entry.item;
			}
		}
		return default(T);
	}

	public Entry GetLastPickedEntry()
	{
		return lastPickedEntry;
	}

	public void Clear()
	{
		entries.Clear();
		totalWeight = 0f;
	}
}
