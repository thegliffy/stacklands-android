public class Shell : CardData
{
	protected override bool CanHaveCard(CardData otherCard)
	{
		if (otherCard.MyCardType != CardType.Resources)
		{
			return otherCard.MyCardType == CardType.Humans;
		}
		return true;
	}
}
