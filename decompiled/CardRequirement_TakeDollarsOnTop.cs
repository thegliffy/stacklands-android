using System;

[Serializable]
public class CardRequirement_TakeDollarsOnTop : CardRequirement
{
	public int Amount;

	public override string RequirementDescriptionNeed(int multiplier)
	{
		string value = $"{Amount * multiplier}";
		return SokLoc.Translate("label_requirement_take_dollars_on_top", LocParam.Create("amount", value));
	}

	public override string RequirementDescriptionNeedNegative(int multiplier)
	{
		string value = $"{Amount * multiplier}";
		return SokLoc.Translate("label_requirement_take_dollars_on_top_negative", LocParam.Create("amount", value));
	}

	public override bool Satisfied(GameCard card)
	{
		return card.CardData.GetDollarCountInStack(includeInChest: true) >= Amount;
	}
}
