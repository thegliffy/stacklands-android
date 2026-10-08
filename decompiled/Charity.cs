using System.Collections.Generic;

public class Charity : CardData
{
	public float GenerationTime = 5f;

	public int RequiredCoins = 3;

	private List<CardData> golds = new List<CardData>();

	public override bool DetermineCanHaveCardsWhenIsRoot => true;

	public override bool CanHaveCardsWhileHasStatus()
	{
		return true;
	}

	protected override bool CanHaveCard(CardData otherCard)
	{
		return otherCard.Id == "gold";
	}

	public override void UpdateCard()
	{
		GetChildrenMatchingPredicate((CardData x) => x is Gold, golds);
		if (golds.Count >= RequiredCoins)
		{
			MyGameCard.StartTimer(GenerationTime, CompleteCharity, SokLoc.Translate("card_charity_status_active"), "complete_charity");
		}
		else
		{
			MyGameCard.CancelTimer("complete_charity");
		}
		base.UpdateCard();
	}

	public override void UpdateCardText()
	{
		descriptionOverride = SokLoc.Translate("card_charity_description", LocParam.Create("amount", RequiredCoins.ToString()));
		base.UpdateCardText();
	}

	[TimedAction("complete_charity")]
	public void CompleteCharity()
	{
		GetChildrenMatchingPredicate((CardData x) => x is Gold, golds);
		if (golds.Count >= RequiredCoins)
		{
			DestroyChildrenMatchingPredicateAndRestack((CardData x) => golds.Contains(x), RequiredCoins);
			WorldManager.instance.TryCreateHappiness(base.transform.position, 1);
		}
	}
}
