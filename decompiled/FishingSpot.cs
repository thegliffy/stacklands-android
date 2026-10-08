public class FishingSpot : Harvestable
{
	public CardBag NormalCardBag;

	public CardBag FisherCardBag;

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (otherCard.Id == "rope" || otherCard.Id == "stone" || otherCard.Id == "sandstone")
		{
			return true;
		}
		return base.CanHaveCard(otherCard);
	}

	public override ICardId GetCardToGive()
	{
		if (HasCardOnTop(out BaseVillager card) && card.Id == "fisher")
		{
			return FisherCardBag.GetCard();
		}
		return NormalCardBag.GetCard();
	}
}
