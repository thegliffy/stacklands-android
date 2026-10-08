public class Spring : Harvestable
{
	public override bool DetermineCanHaveCardsWhenIsRoot => true;

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (otherCard.Id == "empty_bottle" || otherCard.Id == "magic_dust" || otherCard is BaseVillager || otherCard.Id == "brick")
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
