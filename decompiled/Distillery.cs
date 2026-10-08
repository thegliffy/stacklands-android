public class Distillery : CardData
{
	public override bool DetermineCanHaveCardsWhenIsRoot => true;

	public override bool CanHaveCardsWhileHasStatus()
	{
		return true;
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (!(otherCard.Id == "bottle_of_water") && !(otherCard.Id == "sugar"))
		{
			return otherCard.Id == "water";
		}
		return true;
	}
}
