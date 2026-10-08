using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameDataValidator
{
	public GameDataLoader GameDataLoader;

	public GameDataValidator(GameDataLoader gameDataLoader)
	{
		GameDataLoader = gameDataLoader;
	}

	public ValidationResult Validate()
	{
		ValidationResult validationResult = new ValidationResult();
		VerifyBlueprints(validationResult);
		CheckDuplicateBlueprints(validationResult);
		CheckSetCardBags(validationResult);
		CheckCardBags(validationResult);
		VerifyBoosterPacks(validationResult);
		CheckCardTerms(validationResult);
		VerifyAllCardsReferenced(validationResult);
		VerifyQuests(validationResult);
		CalculateExpectedValues(validationResult);
		CheckCardDataUsage(validationResult);
		CheckDefaultAudio(validationResult);
		return validationResult;
	}

	private void CalculateExpectedValues(ValidationResult validationResult)
	{
		foreach (BoosterpackData boosterpackData in GameDataLoader.BoosterpackDatas)
		{
			for (int i = 0; i < boosterpackData.CardBags.Count; i++)
			{
				CardBag cardBag = boosterpackData.CardBags[i];
				try
				{
					cardBag.CalculateExpectedValueForBag(GameDataLoader);
				}
				catch (Exception exception)
				{
					validationResult.AddError(null, $"Error when processing bag {i} in booster {boosterpackData.BoosterId}", ValidationCategory.ExpectedValues);
					Debug.LogException(exception);
				}
			}
			boosterpackData.ExpectedValue = boosterpackData.CardBags.Sum((CardBag x) => x.ExpectedValue * (float)((x.CardBagType == CardBagType.SetPack) ? x.SetPackCards.Count : x.CardsInPack));
		}
	}

	private void VerifyQuests(ValidationResult validationResult)
	{
		foreach (Quest allQuest in QuestManager.GetAllQuests())
		{
			string term = allQuest.DescriptionTerm;
			if (allQuest.DescriptionTermOverride != null)
			{
				term = allQuest.DescriptionTermOverride;
			}
			if (!SokLoc.FallbackSet.ContainsTerm(term))
			{
				validationResult.AddError(null, "Quest " + allQuest.Id + " has an invalid DescriptionTerm (" + allQuest.DescriptionTerm + ")", ValidationCategory.Quests);
			}
		}
	}

	private void CheckCardTerms(ValidationResult validationResult)
	{
		foreach (CardData cardDataPrefab in GameDataLoader.CardDataPrefabs)
		{
			if (!SokLoc.FallbackSet.ContainsTerm(cardDataPrefab.NameTerm))
			{
				validationResult.AddError(cardDataPrefab, cardDataPrefab.Id + " has an invalid NameTerm", ValidationCategory.CardTerms);
			}
			if (!string.IsNullOrWhiteSpace(cardDataPrefab.DescriptionTerm) && !SokLoc.FallbackSet.ContainsTerm(cardDataPrefab.DescriptionTerm))
			{
				validationResult.AddError(cardDataPrefab, cardDataPrefab.Id + " has an invalid DescriptionTerm", ValidationCategory.CardTerms);
			}
		}
	}

	private void VerifyAllCardsReferenced(ValidationResult validationResult)
	{
		List<ICardReference> source = GameDataLoader.DetermineCardReferences();
		foreach (CardData cd in GameDataLoader.CardDataPrefabs)
		{
			if (!(cd is Blueprint { HideFromCardopedia: not false }) && !source.Any((ICardReference x) => x.ReferencedCardId == cd.Id))
			{
				validationResult.AddError(cd, "Card " + cd.Id + " is never referenced", ValidationCategory.CardReferences);
			}
		}
	}

	private void CheckCardDataUsage(ValidationResult validationResult)
	{
		foreach (CardData cardDataPrefab in GameDataLoader.CardDataPrefabs)
		{
			if (!(cardDataPrefab is Blueprint { HideFromCardopedia: not false }) && cardDataPrefab.GetType() == typeof(CardData))
			{
				validationResult.AddError(cardDataPrefab, "Card " + cardDataPrefab.Id + " has base CardData class", ValidationCategory.CardClasses);
			}
		}
	}

	public void CheckStackOrders()
	{
		foreach (Blueprint blueprintPrefab in GameDataLoader.BlueprintPrefabs)
		{
			foreach (Subprint subprint in blueprintPrefab.Subprints)
			{
				CheckSubprintStackOrder(subprint);
			}
		}
	}

	private void CheckSubprintStackOrder(Subprint sp)
	{
		if (sp.RequiredCards.Length >= 7)
		{
			Debug.Log($"Did not check blueprint {sp.ParentBlueprint.Name} subprint {sp.SubprintIndex}");
			return;
		}
		List<CardData> list = new List<CardData>();
		string[] requiredCards = sp.RequiredCards;
		foreach (string text in requiredCards)
		{
			string text2 = text;
			switch (text2)
			{
			case "any_villager":
			case "breedable_villager":
			case "any_villager_old":
			case "any_villager_young":
				text2 = "villager";
				break;
			}
			if (text2 == "any_worker")
			{
				text2 = "worker";
			}
			if (text2.Contains('|'))
			{
				text2 = text.Split('|')[0];
			}
			list.Add(WorldManager.instance.CreateCard(WorldManager.instance.MiddleOfBoard(), text2, faceUp: true, checkAddToStack: false));
		}
		foreach (IEnumerable<CardData> item in list.Permute())
		{
			List<CardData> list2 = item.ToList();
			for (int j = 0; j < list2.Count - 1; j++)
			{
				if (list2[j].CanHaveCardOnTop(list2[j + 1]))
				{
					list2[j].MyGameCard.SetChild(list2[j + 1].MyGameCard);
				}
				else if (list2[j + 1].MyCardType != 0)
				{
					Debug.LogError(list2[j + 1].Id + " can not go on top of " + list2[j].Id + " for blueprint " + sp.ParentBlueprint.Id);
					break;
				}
			}
			foreach (CardData item2 in list2)
			{
				item2.MyGameCard.RemoveFromStack();
			}
		}
		foreach (CardData item3 in list)
		{
			item3.MyGameCard.DestroyCard();
		}
	}

	private void CheckCardBags(ValidationResult validationResult)
	{
		foreach (CardData cardDataPrefab in GameDataLoader.CardDataPrefabs)
		{
			foreach (CardBag cardBag in cardDataPrefab.GetCardBags())
			{
				foreach (string item in cardBag.GetCardsInBag(GameDataLoader))
				{
					if (GameDataLoader.GetCardFromId(item, throwError: false) == null)
					{
						validationResult.AddError(cardDataPrefab, "Invalid card id in " + cardDataPrefab.Id + " " + item, ValidationCategory.CardBag);
					}
				}
			}
		}
	}

	private void VerifyBoosterPacks(ValidationResult validationResult)
	{
		foreach (BoosterpackData boosterpackData in GameDataLoader.BoosterpackDatas)
		{
			for (int i = 0; i < boosterpackData.CardBags.Count; i++)
			{
				foreach (string item in boosterpackData.CardBags[i].GetCardsInBag(GameDataLoader))
				{
					if (GameDataLoader.GetCardFromId(item, throwError: false) == null)
					{
						validationResult.AddError(null, $"Invalid card id in {boosterpackData.BoosterId} - bag {i}: {item}", ValidationCategory.BoosterPacks);
					}
				}
			}
		}
	}

	private void VerifyBlueprints(ValidationResult validationResult)
	{
		foreach (Blueprint blueprintPrefab in GameDataLoader.BlueprintPrefabs)
		{
			if (blueprintPrefab.MyCardType != CardType.Ideas)
			{
				validationResult.AddError(blueprintPrefab, "Blueprint " + blueprintPrefab.Id + " is not set to card type Ideas", ValidationCategory.Blueprints);
			}
			for (int i = 0; i < blueprintPrefab.Subprints.Count; i++)
			{
				Subprint subprint = blueprintPrefab.Subprints[i];
				if (!string.IsNullOrEmpty(subprint.ResultAction))
				{
					continue;
				}
				List<string> list = new List<string>(subprint.ExtraResultCards);
				if (subprint.ExtraResultCards.Length == 0 && subprint.ResultCard != "")
				{
					list.Add(subprint.ResultCard);
				}
				if (subprint.RequiredCards.Length == 0)
				{
					validationResult.AddError(blueprintPrefab, $"Blueprint {blueprintPrefab.Id} has no required cards in subprint {i}", ValidationCategory.Blueprints);
				}
				string[] requiredCards = subprint.RequiredCards;
				for (int j = 0; j < requiredCards.Length; j++)
				{
					string[] collection = requiredCards[j].Split('|');
					list.AddRange(collection);
				}
				if (subprint.CardsToRemove != null)
				{
					requiredCards = subprint.CardsToRemove;
					for (int j = 0; j < requiredCards.Length; j++)
					{
						string[] collection2 = requiredCards[j].Split('|');
						list.AddRange(collection2);
					}
				}
				foreach (string item in list)
				{
					if (GameDataLoader.GetCardFromId(item, throwError: false) == null)
					{
						validationResult.AddError(blueprintPrefab, $"Blueprint {blueprintPrefab.Id} has an invalid card id (id: {item}) (subprint {subprint.SubprintIndex})", ValidationCategory.Blueprints);
					}
				}
				if (!SokLoc.FallbackSet.ContainsTerm(subprint.StatusTerm))
				{
					validationResult.AddError(blueprintPrefab, "Blueprint " + blueprintPrefab.Id + " has an invalid status term", ValidationCategory.Blueprints);
				}
			}
		}
	}

	private void CheckDuplicateBlueprints(ValidationResult validationResult)
	{
		new List<ValidationError>();
		Dictionary<string, Blueprint> dictionary = new Dictionary<string, Blueprint>();
		foreach (Blueprint blueprintPrefab in GameDataLoader.BlueprintPrefabs)
		{
			foreach (Subprint subprint in blueprintPrefab.Subprints)
			{
				string key = string.Join("-", subprint.RequiredCards.OrderBy((string x) => x));
				if (dictionary.ContainsKey(key))
				{
					validationResult.AddError(blueprintPrefab, "Blueprint " + blueprintPrefab.gameObject.name + " has a clashing subprint with " + dictionary[key].gameObject.name, ValidationCategory.BlueprintDuplicates);
				}
				else
				{
					dictionary.Add(key, blueprintPrefab);
				}
			}
		}
	}

	public void CheckSetCardBags(ValidationResult validationResult)
	{
		foreach (SetCardBagData setCardBag in GameDataLoader.SetCardBags)
		{
			SetCardBagType setCardBagType = setCardBag.SetCardBagType;
			foreach (CardChance item in CardBag.GetChancesForSetCardBag(GameDataLoader, setCardBagType))
			{
				if (GameDataLoader.GetCardFromId(item.Id) == null)
				{
					validationResult.AddError(setCardBag, item.Id + " doesn't exist", ValidationCategory.SetCardBag);
				}
			}
		}
	}

	public void CheckDefaultAudio(ValidationResult validationResult)
	{
		foreach (CardData cardDataPrefab in GameDataLoader.CardDataPrefabs)
		{
			if (cardDataPrefab.name.StartsWith("Misc_"))
			{
				continue;
			}
			if (cardDataPrefab.PickupSoundGroup == PickupSoundGroup.Custom)
			{
				if (cardDataPrefab.PickupSound == null)
				{
					validationResult.AddError(cardDataPrefab, "Card " + cardDataPrefab.Id + " has custom sound without an audio source", ValidationCategory.CardAudio);
				}
			}
			else if (!HasCorrectAudio(cardDataPrefab))
			{
				validationResult.AddError(cardDataPrefab, "Card " + cardDataPrefab.Id + " doesn't have the correct audio", ValidationCategory.CardAudio);
			}
		}
	}

	private bool HasCorrectAudio(CardData card)
	{
		if (card.MyCardType == CardType.Structures && card.PickupSoundGroup != PickupSoundGroup.Heavy && card.PickupSoundGroup != PickupSoundGroup.Medium)
		{
			return false;
		}
		if (card.MyCardType == CardType.Food && card.PickupSoundGroup != PickupSoundGroup.Medium)
		{
			return false;
		}
		if (card.MyCardType == CardType.Resources && card.PickupSoundGroup != PickupSoundGroup.Heavy && card.PickupSoundGroup != PickupSoundGroup.Medium)
		{
			return false;
		}
		if (card.MyCardType == CardType.Mobs && card.PickupSoundGroup != 0)
		{
			return false;
		}
		if (card.MyCardType == CardType.Ideas && card.PickupSoundGroup != 0)
		{
			return false;
		}
		if (card.MyCardType == CardType.Equipable && card.PickupSoundGroup != 0)
		{
			return false;
		}
		if (card.MyCardType == CardType.Locations && card.PickupSoundGroup != PickupSoundGroup.Heavy)
		{
			return false;
		}
		return true;
	}
}
