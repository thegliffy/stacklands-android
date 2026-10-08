public class SlaughterHouse : CardData
{
	public override bool DetermineCanHaveCardsWhenIsRoot => true;

	public override bool CanHaveCardsWhileHasStatus()
	{
		return true;
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		int num = GetChildCount() + (1 + otherCard.GetChildCount());
		if (otherCard is Animal)
		{
			return num <= 5;
		}
		return false;
	}

	public override void UpdateCard()
	{
		if (MyGameCard.HasChild && MyGameCard.Child.CardData is Animal)
		{
			MyGameCard.StartTimer(60f, SlaughterAnimal, SokLoc.Translate("action_slaughtering_status"), GetActionId("SlaughterAnimal"));
		}
		else
		{
			MyGameCard.CancelTimer(GetActionId("SlaughterAnimal"));
		}
		base.UpdateCard();
	}

	[TimedAction("slaughter_animal")]
	public void SlaughterAnimal()
	{
		if (MyGameCard.HasChild && MyGameCard.Child.CardData is Animal)
		{
			GameCard child = MyGameCard.Child;
			RemoveFirstChildFromStack();
			child.DestroyCard();
			CardData cardData = ((child.CardData.MyCardType == CardType.Fish) ? WorldManager.instance.CreateCard(base.transform.position, "raw_fish") : ((!(child.CardData.Id == "crab")) ? WorldManager.instance.CreateCard(base.transform.position, "raw_meat") : WorldManager.instance.CreateCard(base.transform.position, "raw_crab_meat")));
			WorldManager.instance.StackSendCheckTarget(MyGameCard, cardData.MyGameCard, OutputDir);
			WorldManager.instance.CreateSmoke(base.transform.position);
		}
	}
}
