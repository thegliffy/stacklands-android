public class Pirate : Enemy
{
	protected override bool CanHaveCard(CardData otherCard)
	{
		if (otherCard.Id == "parrot")
		{
			return true;
		}
		return base.CanHaveCard(otherCard);
	}
}
