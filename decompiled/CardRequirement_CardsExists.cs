using System;

[Serializable]
public class CardRequirement_CardsExists : CardRequirement
{
	[Card]
	public string CardId;

	public int Amount;

	public override string RequirementDescriptionNeed(int multiplier)
	{
		CardData cardPrefab = WorldManager.instance.GetCardPrefab(CardId);
		cardPrefab.UpdateCardText();
		string value = $"{Amount * multiplier}";
		return SokLoc.Translate("label_requirement_take_card", LocParam.Create("amount", value), LocParam.Create("card", cardPrefab.Name));
	}

	public override string RequirementDescriptionNeedNegative(int multiplier)
	{
		CardData cardPrefab = WorldManager.instance.GetCardPrefab(CardId);
		cardPrefab.UpdateCardText();
		string value = $"{Amount * multiplier}";
		return SokLoc.Translate("label_requirement_take_card_negative", LocParam.Create("amount", value), LocParam.Create("card", cardPrefab.Name));
	}

	public override bool Satisfied(GameCard card)
	{
		return WorldManager.instance.GetCards(CardId).Count >= Amount;
	}
}
