using System;

[Serializable]
public class CardRequirement_TakeFood : CardRequirement
{
	public int Amount;

	public override string RequirementDescriptionNeed(int multiplier)
	{
		string value = $"{Amount * multiplier}";
		return SokLoc.Translate("label_requirement_take_food", LocParam.Create("amount", value));
	}

	public override string RequirementDescriptionNeedNegative(int multiplier)
	{
		string value = $"{Amount * multiplier}";
		return SokLoc.Translate("label_requirement_take_food_negative", LocParam.Create("amount", value));
	}

	public override bool Satisfied(GameCard card)
	{
		return CitiesManager.instance.GetFoodToUse(Amount).Count > 0;
	}
}
