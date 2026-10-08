public class IndustrialSmelter : EnergyConsumer
{
	protected override bool CanHaveCard(CardData otherCard)
	{
		if (!(otherCard.Id == "iron_ore") && !(otherCard.Id == "gold_ore") && !(otherCard.Id == "iron_bar") && !(otherCard.Id == "copper_ore") && !(otherCard.Id == "lumber"))
		{
			return otherCard is Worker;
		}
		return true;
	}

	public override void UpdateCard()
	{
		base.UpdateCard();
	}

	public override bool CanHaveCardsWhileHasStatus()
	{
		return true;
	}
}
