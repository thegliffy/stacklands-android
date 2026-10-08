public class CurseHappiness : Curse
{
	protected override bool CanHaveCard(CardData otherCard)
	{
		return otherCard.Id == "euphoria";
	}

	public override void UpdateCard()
	{
		if (MyGameCard.IsDemoCard)
		{
			descriptionOverride = SokLoc.Translate("card_happiness_curse_description");
		}
		else
		{
			descriptionOverride = GameScreen.instance.HappinessSummaryText;
		}
		base.UpdateCard();
	}
}
