public class Brickyard : CardData
{
	protected override bool CanHaveCard(CardData otherCard)
	{
		if (!(otherCard.Id == "stone"))
		{
			return otherCard.Id == "sandstone";
		}
		return true;
	}

	public override void UpdateCard()
	{
		if (ChildrenMatchingPredicateCount((CardData c) => c.Id == "stone" || c.Id == "sandstone") >= 2)
		{
			MyGameCard.StartTimer(10f, CompleteMaking, SokLoc.Translate("card_brickyard_status"), GetActionId("CompleteMaking"));
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
		MyGameCard.GetRootCard().CardData.DestroyChildrenMatchingPredicateAndRestack((CardData c) => c.Id == "stone" || c.Id == "sandstone", 2);
		CardData cardData = WorldManager.instance.CreateCard(base.transform.position, "brick", faceUp: false, checkAddToStack: false);
		WorldManager.instance.StackSendCheckTarget(MyGameCard, cardData.MyGameCard, OutputDir, MyGameCard);
	}
}
