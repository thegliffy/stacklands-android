using System.Collections.Generic;

public class Factory : EnergyConsumer
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
		return base.CanHaveCard(otherCard);
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
