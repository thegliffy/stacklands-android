public class FountainOfYouth : Building
{
	protected override bool CanHaveCard(CardData otherCard)
	{
		if (otherCard is Curse { CurseType: CurseType.Death })
		{
			return true;
		}
		return base.CanHaveCard(otherCard);
	}
}
