using System;

[Serializable]
public class CardRequirement_HasSewerConnected : CardRequirement
{
	public override string RequirementDescriptionNeed(int multiplier)
	{
		return SokLoc.Translate("label_requirement_has_sewer_connected");
	}

	public override string RequirementDescriptionNeedNegative(int multiplier)
	{
		return SokLoc.Translate("label_requirement_has_sewer_connected_negative");
	}

	public override bool Satisfied(GameCard card)
	{
		return card.CardData.HasSewerConnected();
	}
}
