using System.Collections.Generic;

public class Weather : CardData
{
	[Card]
	public List<string> AcceptedCards = new List<string>();

	public override bool DetermineCanHaveCardsWhenIsRoot => true;

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (AcceptedCards.Contains(otherCard.Id))
		{
			return true;
		}
		if (otherCard is Worker)
		{
			return true;
		}
		return otherCard.MyCardType == CardType.Weather;
	}
}
