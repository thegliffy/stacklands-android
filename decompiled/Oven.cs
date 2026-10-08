public class Oven : CardData
{
	public override bool DetermineCanHaveCardsWhenIsRoot => true;

	public override bool CanHaveCardsWhileHasStatus()
	{
		return true;
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (!(otherCard.Id == "dough") && !(otherCard.Id == "cheese"))
		{
			return otherCard.Id == "tomato";
		}
		return true;
	}
}
