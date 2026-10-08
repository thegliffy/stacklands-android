public class Curse : CardData
{
	public CurseType CurseType;

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (!(otherCard.Id == "royal_crown") && !(otherCard.Id == "euphoria"))
		{
			return otherCard.Id == "fountain_of_youth";
		}
		return true;
	}
}
