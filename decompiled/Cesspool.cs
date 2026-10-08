public class Cesspool : CardData
{
	public override bool DetermineCanHaveCardsWhenIsRoot => true;

	public override bool CanHaveCardsWhileHasStatus()
	{
		return true;
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (otherCard is Poop)
		{
			return true;
		}
		return base.CanHaveCard(otherCard);
	}
}
