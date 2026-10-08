using System.Collections.Generic;

public class Barrack : CardData
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
		if (!(otherCard is Worker))
		{
			return otherCard is CitiesCombatable;
		}
		return true;
	}

	public override bool CanHaveCardsWhileHasStatus()
	{
		return true;
	}

	protected override bool CanSelectOutput()
	{
		return true;
	}
}
