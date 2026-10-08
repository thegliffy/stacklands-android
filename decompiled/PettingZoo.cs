public class PettingZoo : CardData
{
	public float GenerationTime;

	public override bool DetermineCanHaveCardsWhenIsRoot => true;

	public override bool CanHaveCardsWhileHasStatus()
	{
		return true;
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		if (otherCard.Id == "soil")
		{
			return true;
		}
		int num = GetChildCount() + (1 + otherCard.GetChildCount());
		if (otherCard is Animal && otherCard.MyCardType != CardType.Fish)
		{
			return num <= 5;
		}
		return false;
	}

	public override void UpdateCard()
	{
		if (ChildrenMatchingPredicateCount((CardData x) => x is Animal) > 0)
		{
			MyGameCard.StartTimer(GenerationTime, CompletePetting, SokLoc.Translate("card_petting_zoo_status_active"), "complete_petting");
		}
		else
		{
			MyGameCard.CancelTimer("complete_petting");
		}
		base.UpdateCard();
	}

	[TimedAction("complete_petting")]
	public void CompletePetting()
	{
		int amount = ChildrenMatchingPredicateCount((CardData x) => x is Animal);
		WorldManager.instance.TryCreateHappiness(MyGameCard.transform.position, amount);
	}
}
