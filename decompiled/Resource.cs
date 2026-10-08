public class Resource : CardData
{
	public override bool CanHaveCardsWhileHasStatus()
	{
		if (IsBuilding)
		{
			return true;
		}
		return base.CanHaveCardsWhileHasStatus();
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (otherCard.MyCardType != CardType.Equipable && otherCard.MyCardType != CardType.Resources && otherCard.MyCardType != CardType.Humans && otherCard.MyCardType != CardType.Food && !(otherCard.Id == Id) && (otherCard.MyCardType != 0 || otherCard.IsBuilding))
		{
			return otherCard.MyCardType == CardType.Weather;
		}
		return true;
	}
}
