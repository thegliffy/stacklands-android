public class ToyFactory : Landmark
{
	protected override bool CanHaveCard(CardData otherCard)
	{
		return otherCard.Id == "plastic";
	}

	public override bool CanHaveCardsWhileHasStatus()
	{
		return true;
	}
}
