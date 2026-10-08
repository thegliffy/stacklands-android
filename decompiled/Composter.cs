public class Composter : CardData
{
	public override bool DetermineCanHaveCardsWhenIsRoot => true;

	public override bool CanHaveCardsWhileHasStatus()
	{
		return true;
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (otherCard is Food { FoodValue: <=0 })
		{
			return false;
		}
		return otherCard.MyCardType == CardType.Food;
	}

	public override void UpdateCard()
	{
		if (ChildrenMatchingPredicateCount((CardData x) => CanHaveCard(x)) >= 5)
		{
			MyGameCard.StartTimer(60f, Compost, SokLoc.Translate("idea_composting_status"), "compost");
		}
		else
		{
			MyGameCard.CancelTimer("compost");
		}
		base.UpdateCard();
	}

	[TimedAction("compost")]
	public void Compost()
	{
		MyGameCard.GetRootCard().CardData.DestroyChildrenMatchingPredicateAndRestack((CardData x) => x.MyCardType == CardType.Food, 5);
		CardData cardData = WorldManager.instance.CreateCard(base.transform.position, "soil", faceUp: false, checkAddToStack: false);
		WorldManager.instance.StackSendCheckTarget(MyGameCard, cardData.MyGameCard, OutputDir);
	}
}
