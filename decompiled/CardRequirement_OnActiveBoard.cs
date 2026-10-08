using System;

[Serializable]
public class CardRequirement_OnActiveBoard : CardRequirement
{
	public override string RequirementDescriptionNeed(int multiplier)
	{
		return SokLoc.Translate("label_requirement_on_active_board");
	}

	public override string RequirementDescriptionNeedNegative(int multiplier)
	{
		return SokLoc.Translate("label_requirement_on_active_board_negative");
	}

	public override bool Satisfied(GameCard card)
	{
		return true;
	}
}
