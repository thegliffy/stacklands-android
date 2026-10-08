using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class SpawnHelper
{
	private class CardIdWithEquipmentCombat
	{
		public List<string> Equipment = new List<string>();

		public float TotalCombatLevel;

		public string Id { get; set; }

		public CardIdWithEquipmentCombat(string id, List<string> equipment, float totalCombatlevel)
		{
			Id = id;
			Equipment = equipment;
			TotalCombatLevel = Mathf.Max(1f, totalCombatlevel);
		}

		public CardIdWithEquipment ToCardIdWithEquipment()
		{
			return new CardIdWithEquipment(Id, Equipment);
		}

		public override string ToString()
		{
			if (Equipment.Count == 0)
			{
				return Id;
			}
			return Id + " (" + string.Join(", ", Equipment) + ")";
		}
	}

	public static CardIdWithEquipment GetEnemyToSpawn(List<SetCardBagType> cardbags, float strength, bool canHaveInventory = true)
	{
		List<CardIdWithEquipmentCombat> allPossibleEnemiesWithEquipment = GetAllPossibleEnemiesWithEquipment(GetEnemyPoolFromCardbags(cardbags, canHaveInventory));
		allPossibleEnemiesWithEquipment.RemoveAll((CardIdWithEquipmentCombat x) => x.TotalCombatLevel > strength);
		allPossibleEnemiesWithEquipment = allPossibleEnemiesWithEquipment.OrderByDescending((CardIdWithEquipmentCombat x) => x.TotalCombatLevel).ToList();
		if (allPossibleEnemiesWithEquipment.Count == 0)
		{
			return null;
		}
		List<string> list = allPossibleEnemiesWithEquipment.Select((CardIdWithEquipmentCombat x) => x.Id).Distinct().ToList();
		string enemyId = list.Choose();
		return allPossibleEnemiesWithEquipment.Where((CardIdWithEquipmentCombat x) => x.Id == enemyId).ToList().Choose()
			.ToCardIdWithEquipment();
	}

	public static List<CardIdWithEquipment> GetEnemiesToSpawn(List<SetCardBagType> cardbags, float strength, bool canHaveInventory = true)
	{
		return GetEnemiesToSpawn(GetEnemyPoolFromCardbags(cardbags, canHaveInventory), strength);
	}

	public static List<Combatable> GetEnemyPoolFromCardbags(List<SetCardBagType> cardbags, bool canHaveInventory)
	{
		List<Combatable> list = new List<Combatable>();
		foreach (SetCardBagType cardbag in cardbags)
		{
			foreach (CardChance item in CardBag.GetChancesForSetCardBag(WorldManager.instance.GameDataLoader, cardbag))
			{
				Combatable combatable = WorldManager.instance.GetCardPrefab(item.Id) as Combatable;
				if (canHaveInventory || !combatable.HasInventory)
				{
					list.Add(combatable);
				}
			}
		}
		return list;
	}

	public static List<CardIdWithEquipment> GetEnemiesToSpawn(List<Combatable> enemyPool, float maxStrength)
	{
		List<CardIdWithEquipmentCombat> list = new List<CardIdWithEquipmentCombat>();
		List<CardIdWithEquipmentCombat> allPossibleEnemiesWithEquipment = GetAllPossibleEnemiesWithEquipment(enemyPool);
		allPossibleEnemiesWithEquipment.RemoveAll((CardIdWithEquipmentCombat x) => x.TotalCombatLevel > maxStrength);
		List<CardIdWithEquipmentCombat> possibleEnemies = new List<CardIdWithEquipmentCombat>(allPossibleEnemiesWithEquipment);
		if (allPossibleEnemiesWithEquipment.Count == 0)
		{
			return new List<CardIdWithEquipment>();
		}
		float num;
		CardIdWithEquipmentCombat cardIdWithEquipmentCombat;
		for (num = 0f; num < maxStrength; num += cardIdWithEquipmentCombat.TotalCombatLevel)
		{
			float leftover = maxStrength - num;
			allPossibleEnemiesWithEquipment.RemoveAll((CardIdWithEquipmentCombat x) => x.TotalCombatLevel > leftover);
			if (allPossibleEnemiesWithEquipment.Count == 0)
			{
				break;
			}
			bool shouldHaveInventory = (double)Random.value <= 0.5;
			List<CardIdWithEquipmentCombat> list2 = allPossibleEnemiesWithEquipment.Where((CardIdWithEquipmentCombat x) => x.Equipment.Count > 0 == shouldHaveInventory).ToList();
			if (list2.Count == 0)
			{
				list2 = allPossibleEnemiesWithEquipment;
			}
			List<string> list3 = list2.Select((CardIdWithEquipmentCombat x) => x.Id).Distinct().ToList();
			string enemyId = list3.Choose();
			cardIdWithEquipmentCombat = allPossibleEnemiesWithEquipment.Where((CardIdWithEquipmentCombat x) => x.Id == enemyId).ToList().Choose();
			list.Add(cardIdWithEquipmentCombat);
		}
		list = OptimizeList(list, possibleEnemies);
		string arg = string.Join(", ", list);
		Debug.Log($"{arg} (combat level: {num})");
		return list.Select((CardIdWithEquipmentCombat x) => x.ToCardIdWithEquipment()).ToList();
	}

	public static CardIdWithEquipment GetEnemyToSpawn(List<Combatable> enemyPool, float maxStrength)
	{
		List<CardIdWithEquipmentCombat> allPossibleEnemiesWithEquipment = GetAllPossibleEnemiesWithEquipment(enemyPool);
		allPossibleEnemiesWithEquipment.RemoveAll((CardIdWithEquipmentCombat x) => x.TotalCombatLevel > maxStrength);
		new List<CardIdWithEquipmentCombat>(allPossibleEnemiesWithEquipment);
		if (allPossibleEnemiesWithEquipment.Count == 0)
		{
			return null;
		}
		bool shouldHaveInventory = (double)Random.value <= 0.5;
		List<CardIdWithEquipmentCombat> list = allPossibleEnemiesWithEquipment.Where((CardIdWithEquipmentCombat x) => x.Equipment.Count > 0 == shouldHaveInventory).ToList();
		if (list.Count == 0)
		{
			list = allPossibleEnemiesWithEquipment;
		}
		List<string> list2 = list.Select((CardIdWithEquipmentCombat x) => x.Id).Distinct().ToList();
		string enemyId = list2.Choose();
		return allPossibleEnemiesWithEquipment.Where((CardIdWithEquipmentCombat x) => x.Id == enemyId).ToList().Choose()
			.ToCardIdWithEquipment();
	}

	private static List<CardIdWithEquipmentCombat> OptimizeList(List<CardIdWithEquipmentCombat> list, List<CardIdWithEquipmentCombat> possibleEnemies)
	{
		possibleEnemies = possibleEnemies.OrderBy((CardIdWithEquipmentCombat x) => x.TotalCombatLevel).ToList();
		int num = 7;
		while (list.Count > num)
		{
			list = list.OrderBy((CardIdWithEquipmentCombat x) => x.TotalCombatLevel).ToList();
			CardIdWithEquipmentCombat cardIdWithEquipmentCombat = list[0];
			CardIdWithEquipmentCombat cardIdWithEquipmentCombat2 = list[1];
			float strength = cardIdWithEquipmentCombat.TotalCombatLevel + cardIdWithEquipmentCombat2.TotalCombatLevel;
			CardIdWithEquipmentCombat enemyWithStrength = GetEnemyWithStrength(possibleEnemies, strength);
			list.Remove(cardIdWithEquipmentCombat);
			list.Remove(cardIdWithEquipmentCombat2);
			list.Add(enemyWithStrength);
		}
		return list;
	}

	private static CardIdWithEquipmentCombat GetEnemyWithStrength(List<CardIdWithEquipmentCombat> possibleEnemies, float strength)
	{
		for (int i = 0; i < possibleEnemies.Count - 1; i++)
		{
			if (possibleEnemies[i + 1].TotalCombatLevel > strength)
			{
				return possibleEnemies[i];
			}
		}
		return possibleEnemies[possibleEnemies.Count - 1];
	}

	private static List<CardIdWithEquipmentCombat> GetAllPossibleEnemiesWithEquipment(List<Combatable> enemyPool)
	{
		List<CardIdWithEquipmentCombat> list = new List<CardIdWithEquipmentCombat>();
		foreach (Combatable item in enemyPool)
		{
			list.Add(new CardIdWithEquipmentCombat(item.Id, new List<string>(), item.RealBaseCombatStats.CombatLevel));
			if (!item.HasInventory)
			{
				continue;
			}
			List<Equipable> equipableOfType = GetEquipableOfType(item.PossibleEquipables, EquipableType.Head);
			equipableOfType.Add(null);
			List<Equipable> equipableOfType2 = GetEquipableOfType(item.PossibleEquipables, EquipableType.Weapon);
			equipableOfType2.Add(null);
			List<Equipable> equipableOfType3 = GetEquipableOfType(item.PossibleEquipables, EquipableType.Torso);
			equipableOfType3.Add(null);
			foreach (Equipable item2 in equipableOfType)
			{
				foreach (Equipable item3 in equipableOfType2)
				{
					foreach (Equipable item4 in equipableOfType3)
					{
						List<string> list2 = new List<string>();
						CombatStats combatStats = new CombatStats();
						combatStats.InitStats(item.RealBaseCombatStats);
						if (item2 != null)
						{
							combatStats.AddStats(item2.MyStats);
							list2.Add(item2.Id);
						}
						if (item3 != null)
						{
							combatStats.AddStats(item3.MyStats);
							list2.Add(item3.Id);
						}
						if (item4 != null)
						{
							combatStats.AddStats(item4.MyStats);
							list2.Add(item4.Id);
						}
						if (list2.Count > 0)
						{
							list.Add(new CardIdWithEquipmentCombat(item.Id, list2, combatStats.CombatLevel));
						}
					}
				}
			}
		}
		return list;
	}

	private static List<Equipable> GetEquipableOfType(List<Equipable> equipables, EquipableType t)
	{
		return equipables.Where((Equipable x) => x.EquipableType == t).ToList();
	}
}
