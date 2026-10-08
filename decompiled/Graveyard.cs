public class Graveyard : Harvestable
{
	protected override bool CanHaveCard(CardData otherCard)
	{
		if (otherCard.Id == "corpse")
		{
			return true;
		}
		return base.CanHaveCard(otherCard);
	}
}
