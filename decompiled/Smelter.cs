public class Smelter : CardData
{
	public override bool DetermineCanHaveCardsWhenIsRoot => true;

	public override bool CanHaveCardsWhileHasStatus()
	{
		return true;
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (otherCard.Id == "iron_ore" || otherCard.Id == "wood" || otherCard.Id == "sand" || otherCard.Id == "gold_ore" || otherCard.Id == "gold" || otherCard.Id == "gold_bar" || otherCard.Id == "glass")
		{
			return true;
		}
		return base.CanHaveCard(otherCard);
	}
}
