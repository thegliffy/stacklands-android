using System;
using System.Collections;

[Serializable]
public class CardRequirementResult_ConsumeFood : CardRequirementResult
{
	public int FoodAmount;

	public override IEnumerator EndOfCutscenePerform(GameCard card)
	{
		return null;
	}

	public override RequirementType GetRequirementType()
	{
		return RequirementType.Food;
	}

	public override IEnumerator Perform(GameCard card)
	{
		yield return CitiesManager.instance.ConsumeFood(FoodAmount, card.Position);
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
