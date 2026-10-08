public class Greenhouse : CardData
{
	protected override bool CanHaveCard(CardData otherCard)
	{
		if (otherCard.MyCardType != CardType.Resources && otherCard.MyCardType != CardType.Humans)
		{
			return otherCard.MyCardType == CardType.Food;
		}
		return true;
	}
}
