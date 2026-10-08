using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Subprint
{
	[HideInInspector]
	public int SubprintIndex;

	[HideInInspector]
	public Blueprint ParentBlueprint;

	[Card]
	public string[] RequiredCards;

	[Card]
	public string[] CardsToRemove;

	public int ResultPolution;

	public int ResultWellbeing;

	[Card]
	public string ResultCard;

	public string ResultAction;

	[Card]
	public string[] ExtraResultCards;

	public float Time = 10f;

	[Term]
	public string StatusTerm;

	[HideInInspector]
	public string statusOverride;

	private List<string> missingCards = new List<string>();

	private static Dictionary<string, string> specialCardIds = new Dictionary<string, string>
	{
		{ "any_villager", "" },
		{ "any_villager_young", "" },
		{ "any_villager_old", "" },
		{ "breedable_villager", "" },
		{ "stone", "stone|sandstone" },
		{ "cotton", "cotton" },
		{ "fish", "cod|eel|mackerel|tuna|shark" },
		{ "any_worker", "" },
		{ "any_educated_worker", "" }
	};

	public string StatusName
	{
		get
		{
			if (!string.IsNullOrEmpty(statusOverride))
			{
				return statusOverride;
			}
			return SokLoc.Translate(StatusTerm);
		}
	}

	public static List<string> GetSpecialCardIds()
	{
		UpdateAnyVillagerCardIds();
		UpdateAnyWorkerCardIds();
		return specialCardIds.Keys.ToList();
	}

	public static void UpdateAnyVillagerCardIds()
	{
		Dictionary<string, string> dictionary = specialCardIds;
		Dictionary<string, string> dictionary2 = specialCardIds;
		string text2 = (specialCardIds["any_villager"] = string.Join("|", GetVillagerCardIds()));
		string value = (dictionary2["any_villager_young"] = text2);
		dictionary["any_villager_old"] = value;
		specialCardIds["breedable_villager"] = string.Join("|", GetVillagerCardIds((BaseVillager x) => x.CanBreed));
	}

	public static void UpdateAnyWorkerCardIds()
	{
		specialCardIds["any_worker"] = string.Join("|", GetWorkerCardIds());
		specialCardIds["any_educated_worker"] = string.Join("|", GetWorkerCardIds((Worker x) => x.WorkerType == WorkerType.Educated || x.WorkerType == WorkerType.Robot));
	}

	private static List<string> GetVillagerCardIds(Predicate<BaseVillager> pred = null)
	{
		List<string> list = new List<string>();
		GameDataLoader gameDataLoader = WorldManager.instance?.GameDataLoader;
		if (gameDataLoader == null)
		{
			gameDataLoader = GameDataLoader.instance;
		}
		foreach (CardData cardDataPrefab in gameDataLoader.CardDataPrefabs)
		{
			if (cardDataPrefab is BaseVillager obj && (pred == null || pred(obj)))
			{
				list.Add(cardDataPrefab.Id);
			}
		}
		return list;
	}

	private static List<string> GetWorkerCardIds(Predicate<Worker> pred = null)
	{
		List<string> list = new List<string>();
		GameDataLoader gameDataLoader = WorldManager.instance?.GameDataLoader;
		if (gameDataLoader == null)
		{
			gameDataLoader = GameDataLoader.instance;
		}
		foreach (CardData cardDataPrefab in gameDataLoader.CardDataPrefabs)
		{
			if (cardDataPrefab is Worker obj && (pred == null || pred(obj)))
			{
				list.Add(cardDataPrefab.Id);
			}
		}
		return list;
	}

	public bool StackMatchesSubprint(GameCard rootCard, out SubprintMatchInfo matchInfo)
	{
		matchInfo = default(SubprintMatchInfo);
		int num = rootCard.GetChildCount() + 1;
		if ((bool)rootCard.HasCardInStack((CardData x) => x.Id == "heavy_foundation"))
		{
			num--;
		}
		if (num < RequiredCards.Length)
		{
			return false;
		}
		if (ParentBlueprint.NeedsExactMatch && num != RequiredCards.Length)
		{
			return false;
		}
		missingCards.Clear();
		missingCards.AddRange(RequiredCards);
		GameCard gameCard = rootCard;
		int num2 = 0;
		while (gameCard != null)
		{
			for (int num3 = missingCards.Count - 1; num3 >= 0; num3--)
			{
				string s = ParseCardId(missingCards[num3]);
				if (CardStringSplitter.me.Split(s).Contains(gameCard.CardData.Id))
				{
					missingCards.RemoveAt(num3);
					break;
				}
			}
			if (missingCards.Count == 0)
			{
				matchInfo = new SubprintMatchInfo(num2, RequiredCards.Length);
				return true;
			}
			gameCard = gameCard.Child;
			num2++;
		}
		return false;
	}

	public List<string> GetAllCardsToRemove()
	{
		List<string> list = new List<string>();
		if (CardsToRemove != null && CardsToRemove.Length != 0)
		{
			string[] cardsToRemove = CardsToRemove;
			foreach (string cardId in cardsToRemove)
			{
				string item = ParseCardId(cardId);
				list.Add(item);
			}
		}
		else
		{
			string[] cardsToRemove = RequiredCards;
			foreach (string cardId2 in cardsToRemove)
			{
				string text = ParseCardId(cardId2);
				string id = CardStringSplitter.me.Split(text)[0];
				CardData cardPrefab = WorldManager.instance.GetCardPrefab(id);
				if (cardPrefab.MyCardType != CardType.Humans && cardPrefab.MyCardType != 0 && cardPrefab.Id != "worker")
				{
					list.Add(text);
				}
			}
		}
		return list;
	}

	private string ParseCardId(string cardId)
	{
		if (specialCardIds.ContainsKey(cardId))
		{
			return specialCardIds[cardId];
		}
		return cardId;
	}

	public string DefaultText()
	{
		List<string> list = RequiredCards.ToList();
		for (int i = 0; i < list.Count; i++)
		{
			string[] array = CardStringSplitter.me.Split(list[i]);
			CardData cardPrefab = WorldManager.instance.GetCardPrefab(array[0]);
			cardPrefab.UpdateCardText();
			list[i] = cardPrefab.Name;
		}
		List<string> list2 = list.Distinct().ToList();
		string text = "";
		for (int j = 0; j < list2.Count; j++)
		{
			string card = list2[j];
			int num = list.Count((string x) => x == card);
			text += $"{num}x {card}";
			if (j < list2.Count - 1)
			{
				text += "\n";
			}
		}
		return text;
	}
}
