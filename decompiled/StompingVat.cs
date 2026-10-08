using System.Collections.Generic;

public class StompingVat : CardData
{
	public List<string> CanHaveIds = new List<string> { "grape", "olive" };

	public override bool DetermineCanHaveCardsWhenIsRoot => true;

	public override bool CanHaveCardsWhileHasStatus()
	{
		return true;
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (otherCard is BaseVillager)
		{
			return true;
		}
		return CanHaveIds.Contains(otherCard.Id);
	}
}
