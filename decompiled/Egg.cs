public class Egg : Food
{
	protected override bool CanHaveCard(CardData otherCard)
	{
		if (otherCard.MyGameCard == null)
		{
			return false;
		}
		if (otherCard.Id == "chicken" && (!MyGameCard.HasParent || MyGameCard.Parent.CardData is HeavyFoundation) && !otherCard.MyGameCard.HasChild)
		{
			return true;
		}
		if (otherCard.Id == "egg" && otherCard.MyGameCard.HasChild && otherCard.MyGameCard.Child.CardData.Id == "chicken")
		{
			return false;
		}
		return base.CanHaveCard(otherCard);
	}
}
