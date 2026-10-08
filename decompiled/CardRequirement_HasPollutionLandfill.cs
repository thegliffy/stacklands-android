using System;

[Serializable]
public class CardRequirement_HasPollutionLandfill : CardRequirement
{
	public int Amount;

	public override string RequirementDescriptionNeed(int multiplier)
	{
		string value = $"{Amount * multiplier}";
		return SokLoc.Translate("label_requirement_has_pollution_landfill", LocParam.Create("amount", value));
	}

	public override string RequirementDescriptionNeedNegative(int multiplier)
	{
		string value = $"{Amount * multiplier}";
		return SokLoc.Translate("label_requirement_has_pollution_landfill_negative", LocParam.Create("amount", value));
	}

	public override bool Satisfied(GameCard card)
	{
		Landfill landfill = card.CardData as Landfill;
		RecyclingCenter recyclingCenter = card.CardData as RecyclingCenter;
		if (landfill != null && landfill.StoredPollution >= Amount)
		{
			return true;
		}
		if (recyclingCenter != null && recyclingCenter.StoredPollution >= Amount)
		{
			return true;
		}
		return false;
	}
}
