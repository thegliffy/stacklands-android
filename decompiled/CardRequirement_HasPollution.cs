using System;

[Serializable]
public class CardRequirement_HasPollution : CardRequirement
{
	public int Amount;

	public override string RequirementDescriptionNeed(int multiplier)
	{
		string value = $"{Amount * multiplier}";
		return SokLoc.Translate("label_requirement_has_pollution", LocParam.Create("amount", value));
	}

	public override string RequirementDescriptionNeedNegative(int multiplier)
	{
		string value = $"{Amount * multiplier}";
		return SokLoc.Translate("label_requirement_has_pollution_negative", LocParam.Create("amount", value));
	}

	public override bool Satisfied(GameCard card)
	{
		if (card.CardData is Pollution pollution && pollution.PollutionAmount >= Amount)
		{
			return true;
		}
		return false;
	}
}
