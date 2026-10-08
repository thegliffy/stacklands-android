using System;

[Serializable]
public class CardRequirement_TakeDollars : CardRequirement
{
	public int Amount;

	public override string RequirementDescriptionNeed(int multiplier)
	{
		string value = $"{Amount * multiplier}";
		return SokLoc.Translate("label_requirement_take_dollars", LocParam.Create("amount", value));
	}

	public override string RequirementDescriptionNeedNegative(int multiplier)
	{
		string value = $"{Amount * multiplier}";
		return SokLoc.Translate("label_requirement_take_dollars_negative", LocParam.Create("amount", value));
	}

	public override bool Satisfied(GameCard card)
	{
		return WorldManager.instance.GetDollarCount(includeInChest: true) >= Amount;
	}
}
