using System.Collections.Generic;

public class FactoryParts : Resource
{
	[Card]
	public List<string> AcceptedCards = new List<string>();

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (AcceptedCards.Contains(otherCard.Id))
		{
			return true;
		}
		return base.CanHaveCard(otherCard);
	}
}
