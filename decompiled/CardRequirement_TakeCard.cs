using System;

[Serializable]
public class CardRequirement_TakeCard : CardRequirement
{
	[Card]
	public string CardId;

	public int Amount;

	public override string RequirementDescriptionNeed(int multiplier)
	{
		return "";
	}

	public override string RequirementDescriptionNeedNegative(int multiplier)
	{
		return "";
	}

	public override bool Satisfied(GameCard card)
	{
		return WorldManager.instance.GetCards(CardId).Count >= Amount;
	}
}
