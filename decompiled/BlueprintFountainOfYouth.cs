using System.Collections.Generic;

public class BlueprintFountainOfYouth : Blueprint
{
	public override void BlueprintComplete(GameCard rootCard, List<GameCard> involvedCards, Subprint print)
	{
		base.BlueprintComplete(rootCard, involvedCards, print);
		foreach (GameCard involvedCard in involvedCards)
		{
			if (involvedCard != null)
			{
				involvedCard.RemoveFromStack();
				involvedCard.SendIt();
			}
		}
	}

	public override Subprint GetMatchingSubprint(GameCard card, out SubprintMatchInfo matchInfo)
	{
		Subprint matchingSubprint = base.GetMatchingSubprint(card, out matchInfo);
		if (matchingSubprint == null)
		{
			return null;
		}
		if (!card.CardData.AnyChildMatchesPredicate((CardData x) => x is BaseVillager baseVillager3 && baseVillager3.MyLifeStage == LifeStage.Teenager) || !card.CardData.AnyChildMatchesPredicate((CardData x) => x is BaseVillager baseVillager2 && baseVillager2.MyLifeStage == LifeStage.Adult) || !card.CardData.AnyChildMatchesPredicate((CardData x) => x is BaseVillager baseVillager && baseVillager.MyLifeStage == LifeStage.Elderly))
		{
			return null;
		}
		return matchingSubprint;
	}
}
