public class Artwork : CardData
{
	protected override bool CanHaveCard(CardData otherCard)
	{
		return Id == otherCard.Id;
	}
}
