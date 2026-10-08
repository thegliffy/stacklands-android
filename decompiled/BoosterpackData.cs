using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Boosterpack", menuName = "ScriptableObjects/Boosterpack", order = 1)]
public class BoosterpackData : ScriptableObject
{
	public string BoosterId;

	public string NameTerm;

	[HideInInspector]
	public string nameOverride;

	public int MinAchievementCount = 3;

	public bool IsIntroPack;

	public int Cost = 3;

	public Sprite Icon;

	public Location BoosterLocation;

	public List<CardBag> CardBags;

	public List<BoosterAddition> BoosterAdditions;

	public float ExpectedValue;

	public string Name
	{
		get
		{
			if (!string.IsNullOrEmpty(nameOverride))
			{
				return nameOverride;
			}
			return SokLoc.Translate(NameTerm);
		}
	}

	public bool IsUnlocked => QuestManager.instance.BoosterIsUnlocked(this);

	public int RemainingAchievementCountToUnlock => QuestManager.instance.RemainingQuestCountToComplete(this);

	public int UndiscoveredCardCount
	{
		get
		{
			List<string> list = new List<string>();
			foreach (CardBag cardBag in CardBags)
			{
				list.AddRange(cardBag.GetCardsInBag());
			}
			return GetUndiscoveredCardCount(list);
		}
	}

	public string GetSummary()
	{
		List<string> list = new List<string>();
		foreach (CardBag cardBag in CardBags)
		{
			list.AddRange(cardBag.GetCardsInBag());
		}
		return GetSummaryFromAllCards(list);
	}

	public static int GetUndiscoveredCardCount(List<string> allCards)
	{
		List<string> list = allCards.Distinct().ToList();
		int num = 0;
		foreach (string item in list)
		{
			if (!WorldManager.instance.CurrentSave.FoundCardIds.Contains(item))
			{
				num++;
			}
		}
		return num;
	}

	public static string GetSummaryFromAllCards(List<string> allCards, string prefix = "label_may_contain")
	{
		if (allCards.Count == 0)
		{
			return "";
		}
		List<string> list = allCards.Distinct().ToList();
		List<string> list2 = new List<string>();
		int num = 0;
		foreach (string item2 in list)
		{
			CardData cardPrefab = WorldManager.instance.GetCardPrefab(item2);
			string item = cardPrefab.FullName;
			if (cardPrefab.MyCardType == CardType.Ideas)
			{
				item = SokLoc.Translate("label_an_idea");
			}
			if (cardPrefab.MyCardType == CardType.Rumors)
			{
				item = SokLoc.Translate("label_a_rumor");
			}
			if (!WorldManager.instance.CurrentSave.FoundCardIds.Contains(item2))
			{
				num++;
			}
			else if (!list2.Contains(item))
			{
				list2.Add(item);
			}
		}
		list2 = (from x in list2
			orderby x
			select "  " + Icons.Circle + " " + x).ToList();
		string text = string.Join("\n", list2);
		string text2 = "";
		if (!string.IsNullOrEmpty(prefix))
		{
			text2 = SokLoc.Translate(prefix) + "\n";
		}
		if (num > 0)
		{
			text2 = text2 + "  " + Icons.Circle + " " + SokLoc.Translate("label_undiscovered_cards", LocParam.Plural("count", num)) + "\n";
		}
		return text2 + text;
	}

	public void LogAllCardsEditor()
	{
		GameDataLoader loader = new GameDataLoader();
		List<string> list = new List<string>();
		foreach (CardBag cardBag in CardBags)
		{
			list.AddRange(cardBag.GetCardsInBag(loader));
		}
		Debug.Log(GetSummaryEditor(list, loader));
	}

	public static string GetSummaryEditor(List<string> allCards, GameDataLoader loader)
	{
		if (allCards.Count == 0)
		{
			return "";
		}
		List<string> list = allCards.Distinct().ToList();
		List<string> list2 = new List<string>();
		foreach (string item2 in list)
		{
			CardData cardFromId = loader.GetCardFromId(item2);
			string item = cardFromId.FullName;
			if (cardFromId.MyCardType == CardType.Ideas)
			{
				item = SokLoc.Translate("label_an_idea");
			}
			if (cardFromId.MyCardType == CardType.Rumors)
			{
				item = SokLoc.Translate("label_a_rumor");
			}
			if (!list2.Contains(item))
			{
				list2.Add(item);
			}
		}
		return string.Join(", ", list2);
	}
}
