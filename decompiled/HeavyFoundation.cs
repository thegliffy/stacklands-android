public class HeavyFoundation : CardData
{
	public override bool DetermineCanHaveCardsWhenIsRoot
	{
		get
		{
			if (!MyGameCard.HasChild)
			{
				return base.DetermineCanHaveCardsWhenIsRoot;
			}
			return MyGameCard.Child.CardData.DetermineCanHaveCardsWhenIsRoot;
		}
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		return true;
	}

	public override bool CanHaveCardsWhileHasStatus()
	{
		if (MyGameCard.HasChild)
		{
			return MyGameCard.Child.CardData.CanHaveCardsWhileHasStatus();
		}
		return base.CanHaveCardsWhileHasStatus();
	}

	public override bool CanBePushedBy(CardData otherCard)
	{
		return otherCard.Id == Id;
	}
}
