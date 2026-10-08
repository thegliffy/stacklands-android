public class Sawmill : CardData
{
	protected override bool CanHaveCard(CardData otherCard)
	{
		return otherCard.Id == "wood";
	}

	public override void UpdateCard()
	{
		if (ChildrenMatchingPredicateCount((CardData c) => c.Id == "wood") >= 2)
		{
			MyGameCard.StartTimer(10f, CompleteMaking, SokLoc.Translate("card_sawmill_status"), GetActionId("CompleteMaking"));
		}
		else
		{
			MyGameCard.CancelTimer(GetActionId("CompleteMaking"));
		}
		base.UpdateCard();
	}

	public override bool CanHaveCardsWhileHasStatus()
	{
		return true;
	}

	[TimedAction("complete_making")]
	public void CompleteMaking()
	{
		MyGameCard.GetRootCard().CardData.DestroyChildrenMatchingPredicateAndRestack((CardData c) => c.Id == "wood", 2);
		CardData cardData = WorldManager.instance.CreateCard(base.transform.position, "plank", faceUp: false, checkAddToStack: false);
		WorldManager.instance.StackSendCheckTarget(MyGameCard, cardData.MyGameCard, OutputDir, MyGameCard);
	}
}
