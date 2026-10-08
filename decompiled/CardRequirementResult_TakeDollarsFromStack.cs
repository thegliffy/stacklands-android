using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class CardRequirementResult_TakeDollarsFromStack : CardRequirementResult
{
	public int Amount;

	public bool IsNegative;

	public override IEnumerator EndOfCutscenePerform(GameCard card)
	{
		return null;
	}

	public override RequirementType GetRequirementType()
	{
		return RequirementType.Dollar;
	}

	public override IEnumerator Perform(GameCard card)
	{
		AudioManager.me.PlaySound2D(AudioManager.me.Dollar, 1f, 0.5f);
		List<GameCard> allCardsInStack = card.GetAllCardsInStack();
		List<ICurrency> currencyList = card.CardData.ChildrenMatchingPredicate((CardData x) => x is ICurrency).Cast<ICurrency>().ToList();
		allCardsInStack.RemoveAll((GameCard x) => currencyList.Select((ICurrency x) => x.Card).Contains(x.CardData));
		CitiesManager.instance.TryUseDollars(currencyList, Amount, onlyTakeIfAmountMet: true);
		allCardsInStack.AddRange(currencyList.Select((ICurrency x) => x.Card.MyGameCard));
		WorldManager.instance.Restack(allCardsInStack);
		card.CardData.UpdateRequirementResultsInStack(RequirementType.Dollar, -Amount, card);
		return null;
	}

	public override string RequirementDescriptionNegative(int multiplier, GameCard card)
	{
		return "";
	}

	public override string RequirementDescriptionPositive(int multiplier, GameCard card)
	{
		return "";
	}
}
