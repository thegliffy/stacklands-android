public class Building : CardData
{
	protected override bool CanHaveCard(CardData otherCard)
	{
		if (otherCard.Id == Id)
		{
			return true;
		}
		return base.CanHaveCard(otherCard);
	}
}
