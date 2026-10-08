public class MessHall : CardData
{
	public override bool DetermineCanHaveCardsWhenIsRoot => true;

	protected override bool CanHaveCard(CardData otherCard)
	{
		return otherCard is Food;
	}
}
