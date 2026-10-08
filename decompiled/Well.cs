public class Well : Harvestable
{
	public override bool DetermineCanHaveCardsWhenIsRoot => true;

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (otherCard.Id == "empty_bottle" || otherCard is BaseVillager || otherCard.Id == "magic_dust" || otherCard.Id == "brick")
		{
			return true;
		}
		return base.CanHaveCard(otherCard);
	}

	public override bool CanHaveCardsWhileHasStatus()
	{
		return true;
	}
}
