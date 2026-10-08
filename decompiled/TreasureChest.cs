using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TreasureChest : CardData
{
	public int Amount = 3;

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (!(otherCard.Id == "key"))
		{
			return otherCard.Id == "treasure_chest";
		}
		return true;
	}

	public override void UpdateCard()
	{
		if (HasCardOnTop("key", out var cardData))
		{
			for (int i = 0; i < Amount; i++)
			{
				WorldManager.instance.CreateCard(base.transform.position, GetCard(), faceUp: false, checkAddToStack: false).MyGameCard.SendIt();
			}
			QuestManager.instance.SpecialActionComplete("treasure_chest_opened", this);
			cardData.MyGameCard.DestroyCard();
			MyGameCard.DestroyCard();
		}
		base.UpdateCard();
	}

	private CardData GetCard()
	{
		List<CardData> list = WorldManager.instance.CardDataPrefabs.Where((CardData x) => (x.MyCardType == CardType.Resources || x.MyCardType == CardType.Food) && x.CardUpdateType == CardUpdateType.Main).ToList();
		list.RemoveAll((CardData x) => x.Id == "goblet");
		return list[Random.Range(0, list.Count)];
	}
}
